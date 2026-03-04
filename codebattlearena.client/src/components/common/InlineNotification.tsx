import { X } from "lucide-react";
import { useEffect } from "react";
import { toast } from "sonner";
import { Button } from "../ui/button";
import { createRoot } from "react-dom/client";

type Position = 'top-left' | 'top-right' | 'bottom-left' | 'bottom-right' | 'top-center' | 'bottom-center';
interface Action {
    label: React.ReactNode;
    onClick: (event: React.MouseEvent<HTMLButtonElement, MouseEvent>) => void;
    actionButtonClassName?: string;
}
interface Props {
    message: string;
    position?: Position;
    className?: string;
    duration?: number; // Infinity → stays forever
    closeButton?: boolean
    action?: Action;
}

export function showInlineNotification({
    message,
    position = "top-center",
    className,
    duration = 7000,
    closeButton = true,
    action,
}: Props): void {
    const container = document.createElement("div");
    document.body.appendChild(container);

    const root = createRoot(container);

    root.render(
        <InlineNotification
            message={message}
            position={position}
            className={className}
            duration={duration}
            closeButton={closeButton}
            action={action}
        />
    );
}

export function InlineNotification({
    message,
    position = "top-center",
    className,
    duration = 7000,
    closeButton = true,
    action,
}: Props) {
    useEffect(() => {
        if (!message) return;
        toast.custom((t) => (
            <div className={`bg-blue border border-1 border-black p-4 rounded-xl min-w-[300px] ${className}`}>
                <div className="flex justify-center items-center">
                    <span className="text-black text-sm font-semibold">{message}</span>
                    {closeButton && (
                        <button
                            onClick={() => toast.dismiss(t)}
                            className="absolute -top-2 -left-2 bg-foreground rounded-full p-1 hover:bg-red transition"
                            aria-label="Close"
                        >
                            <X className="w-4 h-4 text-background" />
                        </button>
                    )}
                    {action && (
                        <Button
                            onClick={action.onClick}
                            className={`btn-animation bg-foreground text-background ml-3 ${action.actionButtonClassName}`}
                        >
                            {action.label}
                        </Button>
                    )}
                </div>
            </div>
        ), {
            position,
            duration,
        });

    }, [message, position, className, duration]);

    return null;
}


export default InlineNotification;