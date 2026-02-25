import axios from 'axios';

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

//  API Gateway or SSO Service URL
const api = axios.create({
    baseURL: '', // Relative URL for Proxy (`/api` will be proxied)
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
        console.log(`API Success: ${response.config.method.toUpperCase()} ${response.config.url}`, response.data);
        return response;
    },
    (error) => {
        const status = error.response?.status;
        // 401 = session invalid/expired, 503 = session validation failed (DB unreachable)
        if (status === 401 || status === 503) {
            console.warn("Session invalid or service unavailable. Logging out...", { status });
            ['token', 'refreshToken', 'authToken', 'userName', 'userRole'].forEach(k => localStorage.removeItem(k));
            ['token', 'authToken'].forEach(k => sessionStorage.removeItem(k));
            window.dispatchEvent(new Event("logout"));
            try { new BroadcastChannel('session-channel').postMessage('logout'); } catch { /* BroadcastChannel not supported */ }

            setTimeout(() => {
                window.location.href = '/login';
            }, 100);
            return Promise.reject(error);
        }

        console.error(`API Error: ${error.config?.method?.toUpperCase?.()} ${error.config?.url}`, {
            status: error.response?.status,
            data: error.response?.data,
            message: error.message
        });
        return Promise.reject(error);
    }
);

export default api;