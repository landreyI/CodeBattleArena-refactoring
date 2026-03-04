import axios from "axios";
import { API_BASE_URL } from "@/config";

export const api = axios.create({
    baseURL: `${API_BASE_URL}/api`,
    headers: {
        "Content-Type": "application/json",
    },
    withCredentials: true
});

// Храним функцию для оверлея 429 ошибки
let showTooManyRequestsOverlay: (() => void) | null = null;
export function registerTooManyRequestsHandler(handler: () => void) {
    showTooManyRequestsOverlay = handler;
}

// Очередь для запросов, которые ждут обновления токена
let isRefreshing = false;
let failedQueue: any[] = [];

const processQueue = (error: any, token: string | null = null) => {
    failedQueue.forEach(prom => {
        if (error) {
            prom.reject(error);
        } else {
            prom.resolve(token);
        }
    });
    failedQueue = [];
};

// 1. Интерцептор запроса: прикрепляем токен
api.interceptors.request.use((config) => {
    const token = localStorage.getItem("accessToken");
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
}, (error) => Promise.reject(error));

// 2. Единый интерцептор ответа
api.interceptors.response.use(
    (response) => response,
    async (error) => {
        const originalRequest = error.config;

        // Обработка Too Many Requests (429)
        if (error.response?.status === 429 && showTooManyRequestsOverlay) {
            showTooManyRequestsOverlay();
            return Promise.reject(error);
        }

        // Если это ошибка 401 и мы НЕ на эндпоинте рефреша
        if (error.response?.status === 401 && !originalRequest._retry && !originalRequest.url.includes("auth/refresh-token")) {

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

            const refreshToken = localStorage.getItem("refreshToken");

            // Если рефреш-токена нет, сразу выходим
            if (!refreshToken) {
                isRefreshing = false;
                clearAuthData();
                return Promise.reject(error);
            }

            try {
                // Используем базовый axios, чтобы не зациклить интерцептор
                const response = await axios.post(`${API_BASE_URL}/api/auth/refresh-token`, {
                    refreshToken: refreshToken
                });

                const { accessToken, newRefreshToken } = response.data;

                localStorage.setItem("accessToken", accessToken);
                localStorage.setItem("refreshToken", newRefreshToken);

                originalRequest.headers.Authorization = `Bearer ${accessToken}`;
                processQueue(null, accessToken);

                return api(originalRequest);
            } catch (refreshError) {
                processQueue(refreshError, null);
                clearAuthData();
                return Promise.reject(refreshError);
            } finally {
                isRefreshing = false;
            }
        }

        return Promise.reject(error);
    }
);

// Функция для очистки данных без жесткого редиректа
function clearAuthData() {
    localStorage.removeItem("accessToken");
    localStorage.removeItem("refreshToken");
    // Если нужно, тут можно дернуть событие, чтобы AuthContext сбросил юзера
    // window.dispatchEvent(new Event("auth-failed"));
}

export default api;