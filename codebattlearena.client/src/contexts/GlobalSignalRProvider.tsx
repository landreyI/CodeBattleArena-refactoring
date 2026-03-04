import React, {
    createContext,
    useContext,
    useEffect,
    useState,
    useCallback,
} from "react";
import * as signalR from "@microsoft/signalr";
import { SIGNALR_MAIN_HUB_URL } from "@/config";

// Контекст для хранения соединения
const GlobalSignalRContext = createContext<signalR.HubConnection | null>(null);

export const GlobalSignalRProvider = ({ children }: { children: React.ReactNode }) => {
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);

    useEffect(() => {
        // Создаем подключение к MainHub
        const conn = new signalR.HubConnectionBuilder()
            .withUrl(SIGNALR_MAIN_HUB_URL, {
                accessTokenFactory: () => {
                    // Берем токен для авторизации
                    const token = localStorage.getItem("accessToken");
                    return token ? token : "";
                }
            })
            .withAutomaticReconnect()
            .build();

        conn.start()
            .then(() => console.log("✅ Global SignalR connected (MainHub)"))
            .catch((err) => console.error("❌ Global SignalR error:", err));

        setConnection(conn);

        // Закрываем соединение при размонтировании всего приложения
        return () => {
            conn.stop();
        };
    }, []);

    return (
        <GlobalSignalRContext.Provider value={connection}>
            {children}
        </GlobalSignalRContext.Provider>
    );
};

/**
 * Хук для доступа к самому объекту соединения.
 * Понадобится, если нужно вызвать метод хаба вручную (например, Invoke).
 */
export function useGlobalSignalR() {
    return useContext(GlobalSignalRContext);
}

/**
 * Универсальный хук для подписки на любые события MainHub.
 * Подойдет для: Presence, Уведомлений, AI-задач.
 * * @param event Название события (например, "ReceiveNotification", "UserOnline", "MetadataReady")
 * @param handler Функция-обработчик
 */
export function useGlobalSignalREvent<TArgs extends any[] = any[]>(
    event: string,
    handler: (...args: TArgs) => void
) {
    const connection = useGlobalSignalR();

    useEffect(() => {
        if (!connection) return;

        // Регистрируем обработчик
        connection.on(event, handler);

        // Очищаем подписку при размонтировании компонента
        return () => {
            connection.off(event, handler);
        };
    }, [connection, event, handler]);
}