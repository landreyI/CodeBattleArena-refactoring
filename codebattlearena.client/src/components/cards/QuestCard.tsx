import { Card, CardContent } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { TaskPlay } from "@/models/dbModels";
import { Coins, Repeat2, Star, Info, Tag } from "lucide-react";

interface Props {
    taskPlay: TaskPlay;
    className?: string;
}

export function QuestCard({ taskPlay, className }: Props) {
    return (
        <Card className={`border p-0 ${className}`}>
            <CardContent className="p-4 flex flex-col gap-3">
                {/* Название задания и повторяемость */}
                <div className="flex flex-col sm:flex-row sm:justify-between sm:items-center gap-2">
                    <div className="font-semibold text-base sm:text-lg break-words">
                        {taskPlay.name || "Unnamed quest"}
                    </div>
                    <Badge
                        className={`w-fit ${taskPlay.isRepeatable ? "bg-green" : "bg-red"}`}
                    >
                        <div className="flex items-center gap-1 text-sm">
                            <Repeat2 className="w-4 h-4" />
                            {taskPlay.isRepeatable ? "Repeatable" : "One-time"}
                        </div>
                    </Badge>
                </div>

                {taskPlay.description && (
                    <p
                        className="text-sm text-muted-foreground"
                        title={taskPlay.description}
                    >
                        {taskPlay.description}
                    </p>
                )}

                <div className="flex items-center gap-2 text-sm text-muted-foreground">
                    <Tag className="w-4 h-4 shrink-0" />
                    <span className="capitalize break-words">{taskPlay.type || "Unknown type"}</span>
                </div>

                <div className="flex flex-wrap items-center justify-start gap-4 text-sm text-muted-foreground mt-2">
                    {taskPlay.rewardCoin != null && (
                        <div className="flex items-center gap-1">
                            <Coins className="w-5 h-5 text-yellow shrink-0" />
                            <span>{taskPlay.rewardCoin}</span>
                        </div>
                    )}
                    {taskPlay.experience != null && (
                        <div className="flex items-center gap-1">
                            <Star className="w-5 h-5 text-purple shrink-0" />
                            <span>{taskPlay.experience} XP</span>
                        </div>
                    )}
                </div>
            </CardContent>
        </Card>
    );
}

export default QuestCard;
