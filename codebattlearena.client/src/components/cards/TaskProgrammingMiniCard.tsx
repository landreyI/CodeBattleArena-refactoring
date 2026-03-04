import { Card, CardContent } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { TaskProgramming } from "@/models/dbModels";
import { getDifficultyColor } from "@/untils/helpers";
import { Link } from "react-router-dom";
import AvatarPlayer from "../avatars/AvatarPlayer";

interface Props {
    task: TaskProgramming;
    className?: string;
    children?: React.ReactNode;
}

export function TaskProgrammingMiniCard({ task, className, children }: Props) {
    return (
        <Card className={`border ${className} flex flex-col md:flex-row md:items-center h-min gap-2 px-6`}>
            <Link to={`/task/info-task/${task.idTaskProgramming}`} title="View Task" >
                <CardContent className="flex flex-wrap items-center gap-2 p-0">
                    <div className="flex gap-2 text-sm flex-col">
                        <div>
                            <span className="font-mono">Name:</span>{" "}
                            <span>{task.name || "Unnamed"}</span>
                        </div>
                        <div>
                            <span className="font-mono">Lang:</span>{" "}
                            <span>{task.langProgramming?.nameLang}</span>
                        </div>
                    </div>

                    <div className="flex md:ml-6 gap-2 h-min">
                        <Badge className={getDifficultyColor(task.difficulty)}>
                            {task.difficulty}
                        </Badge>

                        {task.isGeneratedAI && (
                            <Badge>
                                AI
                            </Badge>
                        )}
                    </div>

                    <div className="w-min">
                        {children}
                    </div>
                </CardContent>
            </Link>

            {task?.player && (
                <Link to={`/player/info-player/${task.player?.id}`} title="View player" className="w-fix">
                    <AvatarPlayer
                        photoUrl={task.player?.photoUrl ?? undefined}
                        username={task.player?.username}
                        className="w-10 h-10"
                        classNameImage="hover:scale-[1.1] transition"
                    />
                </Link>
            )}
        </Card>

    );
};

export default TaskProgrammingMiniCard;