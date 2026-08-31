import axios from 'axios';

// Base URL: '/api' in dev (proxied to the .NET backend by vite.config.js),
// or the real backend URL in production (set via env var at build time).
const baseURL = import.meta.env.VITE_API_URL ?? '/api';

export const apiClient = axios.create({ baseURL });

// Attach the JWT to every outgoing request.
// Token is read fresh from localStorage on each request.
// No need to re-create the client when the user logs in/out
apiClient.interceptors.request.use((config) => {
    const token = localStorage.getItem('authToken');

    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
});

// If the backend ever returns 401 (token expired/invalid),
// clear the stored token so the app knows to show the login screen again instead of silently failing on every subsequent request.
apiClient.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response?.status === 401) {
            localStorage.removeItem('authToken');
        }

        return Promise.reject(error);
    }
);