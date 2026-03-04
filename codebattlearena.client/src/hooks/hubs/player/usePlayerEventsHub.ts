import { Player, Session } from "@/models/dbModels";
import { useGlobalSignalR, useGlobalSignalREvent } from "@/contexts/GlobalSignalRProvider";

interface PlayerEventHandlers {
    onFriendRequest?: (sender: Player) => void;
    onInvitationSession?: (session: Session) => void;
}
export interface ReceiveNotification {
    id: string;
    content: string;
    type: string;
    relatedEntityId?: string;
    createdAt: string;
}

export function usePlayerEventsHub(handlers: PlayerEventHandlers) {
    const connection = useGlobalSignalR();

    useGlobalSignalREvent<[Player]>("FriendRequest", (sender: Player) => {
        handlers.onFriendRequest?.(sender);
    });

    useGlobalSignalREvent<[Session]>("InvitationSession", (session: Session) => {
        handlers.onInvitationSession?.(session);
    });

    useGlobalSignalREvent("UserOnline", (userId: string) => {
        console.log(`User ${userId} has logged in`);
    });

    useGlobalSignalREvent("ReceiveNotification", (notification: ReceiveNotification) => {
       // toast.info(notification.content);
    });
}
