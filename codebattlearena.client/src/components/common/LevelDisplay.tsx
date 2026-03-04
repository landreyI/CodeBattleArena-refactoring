import { Badge } from "@/components/ui/badge";
import { getLevelData } from "@/untils/helpers";
import { Progress } from "../ui/progress";


interface Props {
    experience: number;
}

export function LevelDisplay({ experience }: Props) {
    const { level, currentExp, expToNextLevel, progressPercent } = getLevelData(experience);
    const expLeft = expToNextLevel - currentExp;

    return (
        <div className="flex flex-col items-center gap-1 w-full max-w-sm">
            <Badge className="px-3 py-1 text-sm font-semibold bg-primary-pressed rounded-xl text-white">
                LEVEL {level}
            </Badge>

            <div className="bg-primary-pressed p-1 rounded-xl w-full">
                <Progress value={progressPercent} className="h-4 bg-primary-pressed">
                    <div className="text-xs text-white">
                        {currentExp} / {expToNextLevel}
                    </div>
                </Progress>
            </div>
        </div>
    );
}
