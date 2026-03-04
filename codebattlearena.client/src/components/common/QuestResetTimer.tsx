import { RotateCcw } from "lucide-react";
import { cn } from "@/lib/utils"; // если используешь shadcn cn
import { formatDistanceToNow } from "date-fns";
import { useMemo } from "react";

interface Props {
    completedAt?: Date;
    repeatAfterDays?: number;
    className?: string;
}

export function QuestResetTimer({ completedAt, repeatAfterDays, className }: Props) {
    const resetTime = useMemo(() => {
        if (!completedAt || !repeatAfterDays)
            return null;

        const complAt = new Date(completedAt);
        const resetDate = new Date(complAt);
        resetDate.setDate(resetDate.getDate() + repeatAfterDays);

        return resetDate;
    }, [completedAt, repeatAfterDays]);

    if (!resetTime) return null;

    return (
        <div
            className={cn(
                "flex items-center w-fit gap-2 text-sm text-muted-foreground rounded-2xl bg-muted p-1",
                className
            )}
        >
            <RotateCcw size={16} className="text-primary" />
            <span>
                {formatDistanceToNow(resetTime, { addSuffix: false, locale: undefined })}
            </span>
        </div>
    );
}

export default QuestResetTimer;
