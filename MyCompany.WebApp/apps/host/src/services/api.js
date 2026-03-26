import axios from 'axios';
import { persistRoleFromAccessToken } from '../utils/jwtUtils';

const LS_ACCESS = 'token';
const LS_REFRESH = 'refreshToken';

/**
 * API base URL for axios.
 * - Production / Docker: set VITE_API_BASE_URL=http://localhost:5089 so calls hit the gateway (NOT the static site port 5001).
 * - Dev: leave unset for same-origin + Vite proxy to gateway (see vite.config.js).
 */
function resolveApiBaseUrl() {
    const raw = import.meta.env.VITE_API_BASE_URL;
    if (raw !== undefined && raw !== null && String(raw).trim() !== '') {
        return String(raw).replace(/\/$/, '');
    }
    if (import.meta.env.DEV) {
        return '';
    }
    return '';
}

const baseURL = resolveApiBaseUrl();

const api = axios.create({
    baseURL,
    headers: {
        'Content-Type': 'application/json'
    }
});

api.interceptors.request.use((config) => {
    const token = localStorage.getItem(LS_ACCESS);
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    if (import.meta.env.DEV) {
        const auth = config.headers.Authorization;
        const fullUrl = `${config.baseURL || ""}${config.url || ""}`;
        console.debug("[api] request", config.method?.toUpperCase(), fullUrl, { hasAuth: Boolean(auth) });
    }
    return config;
});

let isRefreshing = false;
let failedQueue = [];

const processQueue = (error, token = null) => {
    failedQueue.forEach(prom => {
        if (error) {
            prom.reject(error);
        } else {
            prom.resolve(token);
        }
    });
    failedQueue = [];
};

/** Refresh access token; one retry on transient errors (no retry on 401/403 invalid refresh). */
async function postRefreshWithRetry(refreshTokenValue) {
    let lastErr;
    for (let attempt = 0; attempt < 2; attempt++) {
        try {
            if (attempt > 0) {
                await new Promise((r) => setTimeout(r, 500));
                console.log("[api] refresh retry (attempt 2)");
            }
            return await api.post("/api/v1/auth/refresh", { refreshToken: refreshTokenValue });
        } catch (e) {
            lastErr = e;
            const st = e.response?.status;
            if (st === 401 || st === 403) throw e;
            if (attempt === 1) throw e;
        }
    }
    throw lastErr;
}

function requestUrlPath(config) {
    if (!config?.url) return '';
    const u = config.url;
    if (u.startsWith('http')) {
        try {
            return new URL(u).pathname;
        } catch {
            return u;
        }
    }
    return u;
}

api.interceptors.response.use(
    (response) => {
        return response;
    },
    async (error) => {
        if (!error.config) {
            return Promise.reject(error);
        }
        const originalRequest = error.config;
        const url = requestUrlPath(originalRequest);
        const urlLower = url.toLowerCase();
        const status = error.response?.status;

        const isLoginRequest = urlLower.includes('/auth/login');
        const isRegisterRequest = urlLower.includes('/auth/register');
        const isRefreshRequest = urlLower.includes('/auth/refresh');
        const isHealthCheck =
            urlLower.includes('/health') || urlLower.includes('/gateway/health');

        // X-Session-Status: MISMATCH = logged in elsewhere, skip refresh and logout immediately
        const sessionStatus = (error.response?.headers?.['x-session-status'] || error.response?.headers?.['x-echo-x-session-status'] || '').toUpperCase();
        const isSessionMismatch = status === 401 && sessionStatus.includes('MISMATCH');

        // 503: do not treat Promotions session-check infra failures (DB_ERROR) as "log out" — that causes instant logout after login when SSO DB is misconfigured.
        // Only treat clear session invalidation signals.
        const isSessionLike503 =
            status === 503 &&
            !isHealthCheck &&
            (sessionStatus.includes('MISMATCH') ||
                (sessionStatus.includes('GATEWAY') && sessionStatus.includes('SESSION')));

        if (isSessionLike503 || isSessionMismatch) {
            console.warn("Session invalid or service unavailable. Logging out.", { status, url, sessionStatus });
            processQueue(error, null);
            isRefreshing = false;
            handleLogout();
            return Promise.reject(error);
        }

        // 401: refresh once, then retry. _retryAfterRefresh marks "already got new access token"; if still 401 → logout.
        if (status === 401 && !isLoginRequest && !isRegisterRequest && !isHealthCheck && !isRefreshRequest) {
            if (originalRequest._retryAfterRefresh) {
                console.warn("[api] 401 still after token refresh; logging out.", { url });
                processQueue(error, null);
                isRefreshing = false;
                handleLogout();
                return Promise.reject(error);
            }

            if (isRefreshing) {
                return new Promise(function (resolve, reject) {
                    failedQueue.push({ resolve, reject });
                })
                    .then((token) => {
                        originalRequest._retryAfterRefresh = true;
                        originalRequest.headers["Authorization"] = "Bearer " + token;
                        return api(originalRequest);
                    })
                    .catch((err) => Promise.reject(err));
            }

            isRefreshing = true;

            const refreshToken = localStorage.getItem(LS_REFRESH);

            if (!refreshToken) {
                console.warn("[api] 401 but no refreshToken; logging out.", { url });
                isRefreshing = false;
                handleLogout();
                return Promise.reject(error);
            }

            try {
                console.log("[api] 401 → attempting token refresh…", { url });
                const response = await postRefreshWithRetry(refreshToken);

                const body = response.data;
                const accessToken = body.accessToken ?? body.AccessToken;
                const newRefreshToken = body.refreshToken ?? body.RefreshToken;
                if (!accessToken) {
                    const missingTokenErr = new Error('Refresh response missing access token');
                    processQueue(missingTokenErr, null);
                    isRefreshing = false;
                    handleLogout();
                    return Promise.reject(missingTokenErr);
                }

                localStorage.setItem(LS_ACCESS, accessToken);
                localStorage.setItem('accessToken', accessToken);
                if (newRefreshToken) {
                    localStorage.setItem(LS_REFRESH, newRefreshToken);
                }
                const roleFromRefresh = (body.role ?? body.Role ?? '').toString().trim();
                persistRoleFromAccessToken(accessToken, roleFromRefresh || undefined);

                api.defaults.headers.common['Authorization'] = `Bearer ${accessToken}`;
                originalRequest.headers['Authorization'] = `Bearer ${accessToken}`;

                processQueue(null, accessToken);
                isRefreshing = false;
                originalRequest._retryAfterRefresh = true;
                return api(originalRequest);
            } catch (refreshError) {
                const st = refreshError.response?.status;
                console.error("[api] refresh failed", { status: st, url }, refreshError);
                processQueue(refreshError, null);
                isRefreshing = false;
                if (st === 401 || st === 403) {
                    handleLogout();
                }
                return Promise.reject(refreshError);
            }
        }

        console.error(`API Error: ${error.config?.method?.toUpperCase()} ${error.config?.url}`, {
            status: error.response?.status,
            data: error.response?.data,
            message: error.message
        });
        return Promise.reject(error);
    }
);

function handleLogout() {
    localStorage.removeItem(LS_ACCESS);
    localStorage.removeItem("accessToken");
    localStorage.removeItem(LS_REFRESH);
    localStorage.removeItem('authToken');
    localStorage.removeItem('userName');
    localStorage.removeItem('userRole');
    sessionStorage.removeItem('token');
    sessionStorage.removeItem('authToken');
    window.dispatchEvent(new Event("logout"));
    try { new BroadcastChannel('session-channel').postMessage('logout'); } catch { /* BroadcastChannel not supported */ }
    setTimeout(() => {
        window.location.href = '/login';
    }, 100);
}

// Multi-tab: when one tab logs out, others follow
try {
    const bc = new BroadcastChannel('session-channel');
    bc.onmessage = (e) => {
        if (e.data === 'logout') {
            localStorage.removeItem(LS_ACCESS);
            localStorage.removeItem("accessToken");
            localStorage.removeItem(LS_REFRESH);
            localStorage.removeItem('authToken');
            localStorage.removeItem('userName');
            localStorage.removeItem('userRole');
            sessionStorage.removeItem('token');
            sessionStorage.removeItem('authToken');
            window.location.href = '/login';
        }
    };
} catch { /* BroadcastChannel not supported */ }

export default api;