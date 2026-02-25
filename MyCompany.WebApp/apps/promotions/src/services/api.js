import axios from 'axios';

// --- Token Refresh State ---
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

// Multi-tab: when one tab logs out, others follow
try {
    const bc = new BroadcastChannel('session-channel');
    bc.onmessage = (e) => {
        if (e.data === 'logout') {
            ['token', 'refreshToken', 'authToken', 'userName', 'userRole'].forEach(k => localStorage.removeItem(k));
            ['token', 'authToken'].forEach(k => sessionStorage.removeItem(k));
            window.location.href = '/login';
        }
    };
} catch { /* BroadcastChannel not supported */ }

const logout = () => {
    console.warn("Session invalid or service unavailable. Logging out...");
    ['token', 'refreshToken', 'authToken', 'userName', 'userRole'].forEach(k => localStorage.removeItem(k));
    ['token', 'authToken'].forEach(k => sessionStorage.removeItem(k));
    window.dispatchEvent(new Event("logout"));
    try { new BroadcastChannel('session-channel').postMessage('logout'); } catch { }

    setTimeout(() => {
        if (!window.location.pathname.includes('/login')) {
            window.location.href = '/login';
        }
    }, 100);
};

const api = axios.create({
    baseURL: '', // Relative URL for Proxy (/api)
    headers: {
        'Content-Type': 'application/json'
    }
});

api.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

api.interceptors.response.use(
    (response) => {
        return response;
    },
    async (error) => {
        const originalRequest = error.config;
        const status = error.response?.status;

        // 1. If 401 and not already retrying
        if (status === 401 && !originalRequest._retry) {

            // Check if we have a refresh token
            const refreshToken = localStorage.getItem('refreshToken');

            // If no refresh token, or if this IS already a refresh call that failed -> Logout
            if (!refreshToken || originalRequest.url.includes('/api/auth/refresh')) {
                logout();
                return Promise.reject(error);
            }

            if (isRefreshing) {
                return new Promise((resolve, reject) => {
                    failedQueue.push({ resolve, reject });
                })
                    .then(token => {
                        originalRequest.headers.Authorization = `Bearer ${token}`;
                        return api(originalRequest);
                    })
                    .catch(err => Promise.reject(err));
            }

            originalRequest._retry = true;
            isRefreshing = true;

            try {
                // Call SSO Refresh
                console.log("[API] Attempting token refresh...");
                const res = await axios.post('/api/auth/refresh', { refreshToken });

                if (res.status === 200 && res.data.accessToken) {
                    const newToken = res.data.accessToken;
                    const newRefresh = res.data.refreshToken;

                    localStorage.setItem('token', newToken);
                    localStorage.setItem('refreshToken', newRefresh);

                    processQueue(null, newToken);

                    // Retry original request
                    originalRequest.headers.Authorization = `Bearer ${newToken}`;
                    return api(originalRequest);
                }
            } catch (refreshError) {
                console.error("[API] Refresh failed:", refreshError);
                processQueue(refreshError, null);
                logout();
                return Promise.reject(refreshError);
            } finally {
                isRefreshing = false;
            }
        }

        // 2. If 503 (DB down during session check), also force logout or show error
        if (status === 503) {
            logout();
        }

        return Promise.reject(error);
    }
);

export default api;