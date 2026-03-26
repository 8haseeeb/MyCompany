import axios from "axios";
import { persistRoleFromAccessToken } from "../utils/jwtUtils";

const LS_ACCESS = "token";
const LS_REFRESH = "refreshToken";

let isRefreshing = false;
let failedQueue = [];

const processQueue = (error, token = null) => {
    failedQueue.forEach((prom) => {
        if (error) prom.reject(error);
        else prom.resolve(token);
    });
    failedQueue = [];
};

async function postRefreshWithRetry(refreshTokenValue) {
    let lastErr;
    for (let attempt = 0; attempt < 2; attempt++) {
        try {
            if (attempt > 0) await new Promise((r) => setTimeout(r, 500));
            // Use default axios so we do not recurse through this instance's interceptors
            return await axios.post("/api/v1/auth/refresh", { refreshToken: refreshTokenValue });
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
    if (!config?.url) return "";
    const u = config.url;
    if (u.startsWith("http")) {
        try {
            return new URL(u).pathname;
        } catch {
            return u;
        }
    }
    return u;
}

try {
    const bc = new BroadcastChannel("session-channel");
    bc.onmessage = (e) => {
        if (e.data === "logout") {
            [LS_ACCESS, "accessToken", LS_REFRESH, "authToken", "userName", "userRole"].forEach((k) =>
                localStorage.removeItem(k)
            );
            ["token", "authToken"].forEach((k) => sessionStorage.removeItem(k));
            window.location.href = "/login";
        }
    };
} catch {
    /* BroadcastChannel not supported */
}

const logout = () => {
    console.warn("[promotions api] logging out");
    [LS_ACCESS, "accessToken", LS_REFRESH, "authToken", "userName", "userRole"].forEach((k) =>
        localStorage.removeItem(k)
    );
    ["token", "authToken"].forEach((k) => sessionStorage.removeItem(k));
    window.dispatchEvent(new Event("logout"));
    try {
        new BroadcastChannel("session-channel").postMessage("logout");
    } catch {
        /* */
    }
    setTimeout(() => {
        if (!window.location.pathname.includes("/login")) {
            window.location.href = "/login";
        }
    }, 100);
};

const api = axios.create({
    baseURL: "",
    headers: {
        "Content-Type": "application/json",
    },
});

api.interceptors.request.use((config) => {
    const token = localStorage.getItem(LS_ACCESS) || localStorage.getItem("accessToken");
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

api.interceptors.response.use(
    (response) => response,
    async (error) => {
        if (!error.config) return Promise.reject(error);

        const originalRequest = error.config;
        const url = requestUrlPath(originalRequest).toLowerCase();
        const status = error.response?.status;

        const isLogin = url.includes("/auth/login");
        const isRegister = url.includes("/auth/register");
        const isRefresh = url.includes("/auth/refresh");
        const isHealth = url.includes("/health") || url.includes("/gateway/health");

        if (status === 401 && !isLogin && !isRegister && !isHealth && !isRefresh) {
            if (originalRequest._retryAfterRefresh) {
                processQueue(error, null);
                isRefreshing = false;
                logout();
                return Promise.reject(error);
            }

            if (isRefreshing) {
                return new Promise((resolve, reject) => {
                    failedQueue.push({ resolve, reject });
                })
                    .then((token) => {
                        originalRequest._retryAfterRefresh = true;
                        originalRequest.headers.Authorization = `Bearer ${token}`;
                        return api(originalRequest);
                    })
                    .catch((err) => Promise.reject(err));
            }

            const refreshToken = localStorage.getItem(LS_REFRESH);
            if (!refreshToken) {
                isRefreshing = false;
                logout();
                return Promise.reject(error);
            }

            isRefreshing = true;
            try {
                const res = await postRefreshWithRetry(refreshToken);
                const body = res.data;
                const newToken = body.accessToken ?? body.AccessToken;
                const newRefresh = body.refreshToken ?? body.RefreshToken;
                if (!newToken) {
                    processQueue(new Error("no access token"), null);
                    isRefreshing = false;
                    logout();
                    return Promise.reject(error);
                }
                localStorage.setItem(LS_ACCESS, newToken);
                localStorage.setItem("accessToken", newToken);
                if (newRefresh) localStorage.setItem(LS_REFRESH, newRefresh);
                persistRoleFromAccessToken(newToken, (body.role ?? body.Role ?? "").toString().trim() || undefined);
                processQueue(null, newToken);
                isRefreshing = false;
                originalRequest._retryAfterRefresh = true;
                originalRequest.headers.Authorization = `Bearer ${newToken}`;
                return api(originalRequest);
            } catch (refreshError) {
                const st = refreshError.response?.status;
                processQueue(refreshError, null);
                isRefreshing = false;
                if (st === 401 || st === 403) {
                    logout();
                }
                return Promise.reject(refreshError);
            }
        }

        return Promise.reject(error);
    }
);

export default api;
