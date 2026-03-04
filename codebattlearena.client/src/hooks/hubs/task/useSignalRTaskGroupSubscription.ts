import { useEffect } from "react";
import { HubConnection } from "@microsoft/signalr";

export function useSignalRTaskGroupSubscription(
    connection: HubConnection | null,
    groupName: string | undefined
) {
    useEffect(() => {
        if (!connection || !groupName) return;

        if (connection.state !== "Connected") {
            console.warn("⚠️ HubConnection not connected yet. Skipping group join.");
            return;
        }

        connection.invoke("JoinTaskGroup", groupName)
            .then(() => console.log(`✅ Joined group ${groupName}`))
            .catch((err) => console.error("❌ Failed to join group:", err));

        return () => {
            if (connection.state === "Connected") {
                connection.invoke("LeaveTaskGroup", groupName)
                    .then(() => console.log(`🛑 Left group ${groupName}`))
                    .catch(console.error);
            }
        };
    }, [connection?.state, groupName]); // Следим за state — это важно!
}
