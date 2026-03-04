import { Card, CardContent } from "@/components/ui/card";
import { Reward } from "@/models/dbModels";
import { Circle, FileText, Tags } from "lucide-react";
import ItemMiniCard from "./ItemMiniCard";

interface Props {
    reward: Reward;
    children?: React.ReactNode;
    className?: string;
}

export function RewardCard({ reward, children, className }: Props) {
    return (
        <>
            <Card className={`border shadow-md w-full ${className}`}>
                <CardContent className="p-4">
                    <div className="grid gap-4 text-sm">
                        {reward?.item && (
                            <div className="flex justify-center">
                                <ItemMiniCard item={reward.item} className="rounded-xl" />
                            </div>
                        )}

                        <div className="flex items-start gap-3">
                            <Tags size={18} className="text-muted-foreground mt-0.5 shrink-0" />
                            <div>
                                <div className="text-xs font-medium text-muted-foreground tracking-wide">
                                    Type reward
                                </div>
                                <div className="whitespace-pre-wrap text-sm">
                                    {reward.rewardType}
                                </div>
                            </div>
                        </div>

                        {reward.amount && (
                            <div className="flex items-start gap-3">
                                <Circle size={18} className="text-muted-foreground mt-0.5 shrink-0" />
                                <div>
                                    <div className="text-xs font-medium text-muted-foreground tracking-wide">
                                        Amount
                                    </div>
                                    <div className="text-base font-medium">
                                        {reward.amount}
                                    </div>
                                </div>
                            </div>
                        )}
                    </div>
                </CardContent>
            </Card>
            {children}
        </>
    );
}

export default RewardCard;
