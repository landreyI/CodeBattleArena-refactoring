import { Card, CardContent } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { TaskProgramming } from "@/models/dbModels";
import { Code2, FileText, Terminal } from "lucide-react";
import { getDifficultyColor } from "@/untils/helpers";
import InputDatasList from "../lists/InputDatasList";
import CodeViewer from "../common/CodeViewer";
import PlayerMiniCard from "./PlayerMiniCard";

interface Props {
    task: TaskProgramming;
    isEditRole?: boolean;
    className?: string;
}

export function TaskProgrammingCard({ task, isEditRole, className }: Props) {
    return (
        <div className="flex flex-col gap-2">
            {task?.player && (
                <div className="flex gap-2 items-center">
                    <span className="text-xl font-bold">Creator: </span>
                    <PlayerMiniCard player={task?.player} className="rounded-3xl hover:scale-[1.02] transition" />
                </div>
            )}
            <Card className={`border w-full ${className}`}>
                <CardContent className="p-6">
                    <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-2 mb-4">
                        <div className="text-xl font-bold font-mono break-words">
                            {task.name || "Unnamed Task"}
                        </div>
                        <div className="flex items-center gap-2">
                            <Badge className={getDifficultyColor(task.difficulty)}>
                                {task.difficulty}
                            </Badge>

                            {task.isGeneratedAI && (
                                <Badge>
                                    Created with AI
                                </Badge>
                            )}
                        </div>

                    </div>

                    <hr className="mb-4" />

                    <div className="flex flex-col gap-4 text-sm w-full">
                        <div className="flex items-start gap-3 w-full">
                            <Code2 size={16} className="mt-1 shrink-0" />
                            <div className="flex items-center gap-2">
                                <div className="font-mono text-muted-foreground">Language:</div>
                                <div className="break-words">{task.langProgramming?.nameLang || "Unknown"}</div>
                            </div>
                        </div>

                        <div className="flex items-start gap-3 w-full">
                            <FileText size={16} className="mt-1 shrink-0" />
                            <div className="flex flex-col w-full">
                                <div className="font-mono text-muted-foreground">Description:</div>
                                <div className="break-words whitespace-pre-wrap">
                                    {task.textTask || "No description"}
                                </div>
                            </div>
                        </div>
                        {isEditRole && (
                            <>
                                <div className="flex gap-3">
                                    <Terminal size={16} className="mt-1 shrink-0" />
                                    <div className="flex flex-col w-full">
                                        <div className="font-mono text-muted-foreground">Preparation:</div>
                                        <CodeViewer code={task.preparation} autoResize={true} language={task.langProgramming?.codeNameLang || "javascript"} className="rounded-xl" />
                                    </div>
                                </div>

                                <div className="flex gap-3">
                                    <Terminal size={16} className="mt-1 shrink-0" />
                                    <div className="flex flex-col w-full">
                                        <div className="font-mono text-muted-foreground">Verification Code:</div>
                                        <CodeViewer code={task.verificationCode} autoResize={true} language={task.langProgramming?.codeNameLang || "javascript"} className="rounded-xl" />
                                    </div>
                                </div>
                            </>
                        )}
                        {task.taskInputData && (
                            <InputDatasList inputDatas={task.taskInputData}></InputDatasList>
                        )}

                    </div>

                </CardContent>
            </Card>
        </div>
    );
}

export default TaskProgrammingCard;
