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
        const isLoginRequest = url.includes('/api/auth/login');
        const isHealthCheck = url.includes('/api/gateway/health');
        const isRefreshRequest = url.includes('/api/auth/refresh');

        // Response Interceptor (Detecting 401)
        if (error.response?.status === 401 && !isLoginRequest && !isHealthCheck && !isRefreshRequest && !originalRequest._retry) {

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
                // Note: We use a separate axios call or the same api instance but must be careful about infinite loops
                // Since it's a specific endpoint, our isRefreshRequest check above handles it.
                const response = await axios.post('/api/auth/refresh', { refreshToken });

                const { accessToken, refreshToken: newRefreshToken } = response.data;

                localStorage.setItem('token', accessToken);
                localStorage.setItem('refreshToken', newRefreshToken);

                api.defaults.headers.common['Authorization'] = 'Bearer ' + accessToken;
                originalRequest.headers['Authorization'] = 'Bearer ' + accessToken;

                processQueue(null, accessToken);
                return api(originalRequest);
            } catch (refreshError) {
                console.error("Refresh token failed or expired. Logging out.", refreshError);
                processQueue(refreshError, null);
                handleLogout();
                return Promise.reject(refreshError);
            } finally {
                isRefreshing = false;
            }
        }

        console.error(`API Error: ${error.config?.method.toUpperCase()} ${error.config?.url}`, {
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
    window.dispatchEvent(new Event("logout"));
    setTimeout(() => {
        window.location.href = '/login';
    }, 100);
}

export default api;