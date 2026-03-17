import axios from 'axios';

// Use env for API base URL: empty = same origin (dev with Vite proxy); set in production (e.g. VITE_API_BASE_URL=https://promo.azure-api.net)
const baseURL = import.meta.env.VITE_API_BASE_URL ?? '';

const api = axios.create({
    baseURL,
    headers: {
        'Content-Type': 'application/json'
    }
});

api.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }

    // When using Azure API Management with path-based backends, set VITE_APIM_PATH_PREFIX=true
    // to send /sso/api/auth and /promotion/api/... to APIM. For direct Gateway (or dev proxy), use /api/... as-is.
    if (import.meta.env.VITE_APIM_PATH_PREFIX === 'true' && config.url && !config.url.startsWith('http')) {
        if (config.url.includes('/api/auth')) {
            config.url = `/sso${config.url}`;
        } else if (config.url.includes('/api/')) {
            config.url = `/promotion${config.url}`;
        }
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

api.interceptors.response.use(
    (response) => {
        return response;
    },
    async (error) => {
        const originalRequest = error.config;
        const url = originalRequest?.url || '';
        const status = error.response?.status;
        const isLoginRequest = url.includes('/api/auth/login');
        const isHealthCheck = url.includes('/api/Health');
        const isRefreshRequest = url.includes('/api/auth/refresh');

        // 401 after retry = session invalidated (e.g. logged in elsewhere). Do NOT try refresh again.
        const is401AfterRetry = status === 401 && originalRequest._retry;
        // X-Session-Status: MISMATCH = logged in elsewhere, skip refresh and logout immediately
        const sessionStatus = (error.response?.headers?.['x-session-status'] || error.response?.headers?.['x-echo-x-session-status'] || '').toUpperCase();
        const isSessionMismatch = status === 401 && sessionStatus.includes('MISMATCH');

        // 503 = session validation failed (DB unreachable). Force logout.
        if (status === 503 || is401AfterRetry || isSessionMismatch) {
            console.warn("Session invalid or service unavailable. Logging out.", { status, url });
            processQueue(error, null);
            isRefreshing = false;
            handleLogout();
            return Promise.reject(error);
        }

        // Response Interceptor (Detecting 401) - try refresh for token expiry
        if (status === 401 && !isLoginRequest && !isHealthCheck && !isRefreshRequest) {

            // Request Queue (Smart Handling)
            if (isRefreshing) {
                return new Promise(function (resolve, reject) {
                    failedQueue.push({ resolve, reject });
                })
                    .then(token => {
                        originalRequest.headers['Authorization'] = 'Bearer ' + token;
                        return api(originalRequest);
                    })
                    .catch(err => {
                        return Promise.reject(err);
                    });
            }

            originalRequest._retry = true;
            isRefreshing = true;

            const refreshToken = localStorage.getItem('refreshToken');

            if (!refreshToken) {
                handleLogout();
                return Promise.reject(error);
            }

            // Refresh Logic
            try {
                console.log("Access token expired. Attempting refresh...");
                const response = await api.post('/api/auth/refresh', { refreshToken });

                const { accessToken, refreshToken: newRefreshToken } = response.data;

                localStorage.setItem('token', accessToken);
                localStorage.setItem('refreshToken', newRefreshToken);

                api.defaults.headers.common['Authorization'] = 'Bearer ' + accessToken;
                originalRequest.headers['Authorization'] = 'Bearer ' + accessToken;

                processQueue(null, accessToken);
                return api(originalRequest);
            } catch (refreshError) {
                // Refresh failed (401/503) = session invalid, force logout
                console.error("Refresh token failed or expired. Logging out.", refreshError);
                processQueue(refreshError, null);
                handleLogout();
                return Promise.reject(refreshError);
            } finally {
                isRefreshing = false;
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
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
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
            localStorage.removeItem('token');
            localStorage.removeItem('refreshToken');
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