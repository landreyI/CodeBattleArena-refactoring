import { Card, CardContent } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { TaskPlay } from "@/models/dbModels";
import { Coins, Repeat2, Star } from "lucide-react";
import { Link } from "react-router-dom";

interface Props {
    taskPlay: TaskPlay;
    className?: string;
    children?: React.ReactNode;
}

export function QuestMiniCard({ taskPlay, className, children }: Props) {
    return (
        <Link to={`/quest/info-quest/${taskPlay.idTask}`} title="View Quest">
            <Card className={`border rounded-2xl p-0 ${className}`}>
                <CardContent className="p-4 flex flex-col gap-2">
                    {/* Название задания */}
                    <div className="flex flex-col sm:flex-row sm:justify-between sm:items-center gap-2">
                        <div className="font-semibold text-base break-words whitespace-normal sm:max-w-[60%]">
                            {taskPlay.name || "Unnamed quest"}
                        </div>

                        <Badge className={taskPlay.isRepeatable ? "bg-green" : "bg-red"} variant="outline">
                            <div className="flex items-center gap-1">
                                <Repeat2 className="w-3 h-3" />
                                {taskPlay.isRepeatable ? "Repeatable" : "One-time"}
                            </div>
                        </Badge>
                    </div>

                    {/* Награда и опыт */}
                    <div className="flex items-center justify-between text-sm text-muted-foreground">
                        {taskPlay.rewardCoin && (
                            <div className="flex items-center gap-1">
                                <Coins className="w-4 h-4 text-yellow-400" />
                                <span>{taskPlay.rewardCoin}</span>
                            </div>
                        )}
                        {taskPlay.experience && (
                            <div className="flex items-center gap-1">
                                <Star className="w-4 h-4 text-purple-400" />
                                <span>{taskPlay.experience} XP</span>
                            </div>
                        )}
                    </div>

                    {children}
                </CardContent>
            </Card>
        </Link>
    );
}

export default QuestMiniCard;