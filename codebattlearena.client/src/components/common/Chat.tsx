import { Button } from "../ui/button";
import { Textarea } from "../ui/textarea";
import { ScrollArea, ScrollBar } from "../ui/scroll-area";
import { MessageComponent, MessageProps } from "./Message";
import { Separator } from "../ui/separator";
import { useEffect, useRef, useState } from "react";
import { ChevronDown, Send } from "lucide-react";
interface ChatProps {
    messages?: MessageProps[];
    onSend?: (message: string) => void;
}

export function Chat({ messages = [], onSend }: ChatProps) {
    const [message, setMessage] = useState("");
    const scrollRef = useRef<HTMLDivElement>(null);
    const containerRef = useRef<HTMLDivElement>(null);
    const [isAtBottom, setIsAtBottom] = useState(true);
    const viewportRef = useRef<HTMLDivElement>(null);

    const handleSendMessage = () => {
        if (message.trim() && onSend) {
            onSend(message);
            setMessage("");
        }
    };

    const scrollToBottom = () => {
        scrollRef.current?.scrollIntoView({ behavior: "smooth" });
    };

    const handleScroll = () => {
        if (!containerRef.current) return;
        const { scrollTop, scrollHeight, clientHeight } = containerRef.current;
        const bottomThreshold = 100; // px
        const isBottom = scrollHeight - scrollTop - clientHeight < bottomThreshold;
        setIsAtBottom(isBottom);
    };

    useEffect(() => {
        if (isAtBottom) scrollToBottom();
    }, [messages]);

    return (
        <div className="flex flex-col h-full overflow-hidden bg-background shadow-sm relative rounded-b-2xl">

            <ScrollArea className="flex-1 overflow-y-auto p-3 bg-muted">
                <div
                    className="ScrollAreaViewport" // если shadcn, это просто div внутри
                    ref={viewportRef}
                    onScroll={handleScroll}
                >
                    <div className="flex flex-col gap-2">
                        {messages.map((msg, index) => (
                            <MessageComponent
                                key={index}
                                message={msg.message}
                                isUser={msg.isUser}
                            />
                        ))}
                        <div ref={scrollRef} />
                    </div>
                </div>
                <ScrollBar orientation="vertical" />
            </ScrollArea>

            {!isAtBottom && (
                <button
                    className="absolute bottom-20 right-4 z-10 p-2 rounded-full bg-primary text-white shadow-md hover:bg-primary/90 transition"
                    onClick={scrollToBottom}
                >
                    <ChevronDown className="w-5 h-5" />
                </button>
            )}

            <Separator />
            <div className="p-3 bg-[var(--color-header-bg)]">
                <div className="flex items-center gap-2">
                    <Textarea
                        placeholder="Type your message..."
                        value={message}
                        onChange={(e) => setMessage(e.target.value)}
                        rows={1}
                        className="w-full max-h-[4rem] overflow-y-auto resize-none rounded-2xl border focus:ring-2 focus:ring-primary transition leading-tight min-h-[2.5rem]"
                        onKeyDown={(e) => {
                            if (e.key === "Enter" && !e.shiftKey) {
                                e.preventDefault();
                                handleSendMessage();
                            }
                        }}
                    />
                    <Button
                        onClick={handleSendMessage}
                        size="icon"
                        variant="ghost"
                        className="rounded-2xl hover:bg-accent transition"
                    >
                        <Send className="w-5 h-5" />
                    </Button>
                </div>
            </div>
        </div>
    );
}

export default Chat;