import { api } from "../api/axios";

/** Запрашиваем URL для OAuth‑редиректа (учитываем AbortSignal) */
export const fetchRedirectOnOauthServer = async (
    _?: void,
    config?: { signal?: AbortSignal }
): Promise<{ url: string }> => {
    const response = await api.get("/googleAuth/google-url", {
        signal: config?.signal,
    });
    return response.data;
};

/** Стартуем вход через Google  */
export const handleGoogleLogin = async () => {
    try {
        const dataUrl = await fetchRedirectOnOauthServer();
        if (dataUrl?.url) {
            window.location.href = dataUrl.url;
        } else {
            console.error("No valid redirect URL received");
            window.location.href = "/login-error";
        }
    } catch (error) {
        console.error("Google login initiation failed:", error);
        window.location.href = "/login-error";
    }
};
