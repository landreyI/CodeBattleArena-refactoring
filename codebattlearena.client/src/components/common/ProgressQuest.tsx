import { PlayerTaskPlay } from "@/models/dbModels";
import { CheckCircle, Clock } from "lucide-react";
import { getProgressDisplay } from "@/untils/helpers";

interface Props {
    playerProgress?: PlayerTaskPlay;
    taskPlayParamValue?: string;
    className?: string;
}
export function ProgressQuest({ playerProgress, taskPlayParamValue, className }: Props) {
    return (
        <div className={`flex items-center justify-start gap-2 my-2 rounded-2xl text-sm text-muted-foreground border border-border bg-muted p-1 ${className}`}>
            <div className="flex items-center gap-1">
                {playerProgress?.isCompleted ? (
                    <CheckCircle className="w-4 h-4 text-green" />
                ) : (
                    <Clock className="w-4 h-4 text-yellow" />
                )}
            </div>

            <span className="font-medium">
                {getProgressDisplay(playerProgress?.progressValue?.toLowerCase(), taskPlayParamValue)}
            </span>
        </div>
    )
}

export default ProgressQuest;