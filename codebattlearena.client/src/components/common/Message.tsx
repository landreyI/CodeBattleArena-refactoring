import { cn } from "@/lib/utils";
import { Avatar, AvatarImage, AvatarFallback } from "@radix-ui/react-avatar";
import { Message } from "@/models/dbModels";
import { Link } from "react-router-dom";

export interface MessageProps {
    message?: Message;
    isUser?: boolean;
}
export function MessageComponent({ message, isUser }: MessageProps) {
    return (
        <div
            className={cn(
                "flex items-start gap-3 mb-4",
                isUser ? "justify-end" : "justify-start"
            )}
        >
            {!isUser && (
                <Link to={`/player/info-player/${message?.idSender}`} title="View player" className="w-10 h-10 sm:w-12 sm:h-12">
                    <Avatar className="">
                        <AvatarImage className="rounded-full" src={message?.sender?.photoUrl ?? undefined} />
                        <AvatarFallback className="flex items-center justify-center h-full rounded-full bg-[var(--color-header-bg)] text-primary">
                            {message?.sender?.name.charAt(0).toUpperCase()}
                        </AvatarFallback>
                    </Avatar>
                </Link>
            )}
            <div
                className={cn(
                    "rounded-xl px-4 py-2",
                    isUser
                        ? "bg-primary text-primary-foreground"
                        : "bg-blue text-primary-foreground"
                )}
            >
                <p className="whitespace-pre-wrap font-bold break-words overflow-auto max-w-60">
                    {message?.messageText}
                </p>
            </div>
            {isUser && (
                <Avatar className="w-10 h-10 sm:w-12 sm:h-12">
                    <AvatarImage className="rounded-full" src={message?.sender?.photoUrl ?? undefined} />
                    <AvatarFallback className="flex items-center justify-center h-full rounded-full bg-[var(--color-header-bg)] text-primary">
                        {message?.sender?.name.charAt(0).toUpperCase()}
                    </AvatarFallback>
                </Avatar>
            )}
        </div>
    );
}

export default MessageComponent;