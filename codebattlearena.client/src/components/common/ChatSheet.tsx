import {
    Sheet,
    SheetContent,
    SheetHeader,
    SheetTitle,
    SheetTrigger,
} from "@/components/ui/sheet";
import { ReactNode } from "react";
import { Chat } from "./Chat";
import { MessageProps } from "./Message";
import { useSessionHubConnection } from "@/contexts/SignalRSessionHubContext";

interface ChatSheetProps {
    trigger: ReactNode;
    sessionId: string;
    messages?: MessageProps[];
    title?: string;
}

export function ChatSheet({ trigger, sessionId, messages, title = "Group Chat" }: ChatSheetProps) {
    const connection = useSessionHubConnection();

    const onSend = async (message: string) => {
        if (!message.trim()) return;

        if (connection?.state === "Connected" && sessionId) {
            try {
                // Вызываем метод прямо в C# хабе
                // "SendMessage" — имя метода в твоем SessionHub.cs
                await connection.invoke("SendMessage", sessionId, message);
            } catch (err) {
                console.error("SignalR: Failed to send message:", err);
            }
        } else {
            console.warn("SignalR: Connection is not active");
        }
    };

    return (
        <Sheet modal={false} defaultOpen={false}>
            <SheetTrigger asChild>
                {trigger}
            </SheetTrigger>
            <SheetContent
                side="right"
                onInteractOutside={(e) => e.preventDefault()}
                className="md:h-[calc(100vh-40px)] md:my-5 md:mr-5 w-full rounded-2xl sm:w-[100vw] border border-2 bg-[var(--color-header-bg)]"
            >
                <SheetHeader>
                    <SheetTitle className="font-semibold text-lg">{title}</SheetTitle>
                </SheetHeader>
                <Chat messages={messages} onSend={onSend}></Chat>
            </SheetContent>
        </Sheet>
    );
};

export default ChatSheet;