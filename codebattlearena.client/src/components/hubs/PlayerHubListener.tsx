import { usePlayerEventsHub } from "@/hooks/hubs/player/usePlayerEventsHub";
import { Player, Session } from "@/models/dbModels";
import { useState } from "react";
import InlineNotification, { showInlineNotification } from "../common/InlineNotification";
import { useAuth } from "@/contexts/AuthContext";
import { useNavigate } from "react-router-dom";


export function PlayerHubListener() {
    const [notification, setNotification] = useState<string | null>(null);
    const { user } = useAuth();
    const navigate = useNavigate();

    usePlayerEventsHub({
        onFriendRequest: (sender: Player) => {
            setNotification(null);
            setNotification(`Player ${sender.username} wants to add you as a friend`);
        },
        onInvitationSession: (session: Session) => {
            showInlineNotification({
                message: `I invite you to the session "${session.name}", ${session.langProgramming?.nameLang}`,
                duration: Infinity,
                action: {
                    label: "join",
                    onClick: () => navigate(`/session/info-session/${session?.idSession}`),
                },
            });
        }
    })

    if (!user)
        return null;

    return (
        <>
            {notification && (
                <InlineNotification message={notification} className="bg-blue" />
            )}
        </>
    )
}

export default PlayerHubListener;