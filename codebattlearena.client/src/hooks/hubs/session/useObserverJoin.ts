import { useEffect } from "react";
import { HubConnectionState } from "@microsoft/signalr";
import { useSessionHubConnection } from "@/contexts/SignalRSessionHubContext";

/**
 * Дожидается подключения SignalR до состояния Connected
 */
async function waitForConnection(connection: signalR.HubConnection): Promise<void> {
    const maxRetries = 50; // максимум ~5 секунд
    let retries = 0;

    while (connection.state !== HubConnectionState.Connected && retries < maxRetries) {
        await new Promise((res) => setTimeout(res, 100));
        retries++;
    }

    if (connection.state !== HubConnectionState.Connected) {
        throw new Error("SignalR connection is not in 'Connected' state.");
    }
}

export function useObserverJoin(sessionId: string | undefined) {
    const connection = useSessionHubConnection();

    useEffect(() => {
        if (!connection || !sessionId) return;

        let isActive = true;

        const join = async () => {
            try {
                await waitForConnection(connection);
                if (!isActive) return;

                await connection.invoke("JoinAsObserver", sessionId);
            } catch (err) {
                console.error("❌ JoinAsObserver error:", err);
            }
        };

        join();

        return () => {
            isActive = false;

            if (connection.state === HubConnectionState.Connected) {
                connection
                    .invoke("LeaveAsObserver", sessionId)
                    .catch((err) =>
                        console.error("❌ LeaveAsObserver error:", err)
                    );
            }
        };
    }, [connection, sessionId]);
}
