import axios from 'axios';

const api = axios.create({
    baseURL: '',
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
        // Don't trigger global logout for login attempts or background health checks
        const url = error.config?.url || '';
        const isLoginRequest = url.includes('/api/auth/login');
        const isHealthCheck = url.includes('/api/gateway/health');

        if (error.response?.status === 401 && !isLoginRequest && !isHealthCheck) {
            // Check if the token sent in the request is the current token.
            // If they don't match, it's a stale request from a previous session being ignored.
            const currentToken = localStorage.getItem('token');
            const requestToken = error.config?.headers?.Authorization?.replace('Bearer ', '');

            if (currentToken && requestToken && currentToken !== requestToken) {
                console.warn(`401 detected for stale token at ${url}. Ignoring logout.`);
                return Promise.reject(error);
            }

            console.error(`Unauthorized (401) at ${url}. Logging out...`);
            localStorage.removeItem('token');
            localStorage.removeItem('refreshToken');
            window.dispatchEvent(new Event("logout"));

            setTimeout(() => {
                window.location.href = '/login';
            }, 100);
        }

        console.error(`API Error: ${error.config?.method.toUpperCase()} ${error.config?.url}`, {
            status: error.response?.status,
            data: error.response?.data,
            message: error.message
        });
        return Promise.reject(error);
    }
);

export default api;