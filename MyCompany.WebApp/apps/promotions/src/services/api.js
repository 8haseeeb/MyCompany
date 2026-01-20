import axios from 'axios';

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
        if (error.response?.status === 401) {
            console.error("Session expired or unauthorized. Logging out...");
            localStorage.removeItem('token');
            // Force a reload to the home page to reset App state
            window.location.href = '/';
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