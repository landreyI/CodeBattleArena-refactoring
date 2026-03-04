import { useState } from "react";
import InlineNotification from "@/components/common/InlineNotification";
import AIGeneratedForm from "@/components/forms/AIGeneratedTaskForm";
import TaskProgrammingCard from "@/components/cards/TaskProgrammingCard";
import { isEditRole } from "@/untils/businessRules";
import { useAuth } from "@/contexts/AuthContext";
import ScreenBlocker from "@/components/common/ScreenBlocker";
import { Link, useParams } from "react-router-dom";
import { useTask } from "@/hooks/task/useTask";
import LoadingScreen from "@/components/common/LoadingScreen";
import ErrorMessage from "@/components/common/ErrorMessage";
import { Button } from "@/components/ui/button";

export function AIGenerateTask() {
    const { taskId } = useParams<{ taskId: string }>();
    const { task, setTask, loading: taskLoad, error: taskError } = useTask(Number(taskId) ?? undefined);

    const [notification, setNotification] = useState<string | null>(null);
    const { user } = useAuth();
    const [loading, setLoading] = useState(false);

    const handleMessageSaccess = () => {
        setNotification(null);
        setNotification("Success");
    }

    if (taskLoad) return <LoadingScreen />

    return (
      <>
            {notification && (
                <InlineNotification message={notification} className="bg-blue" />
            )}

            <ScreenBlocker open={loading} />

            <div className="glow-box">
                <div className="md:w-[50vw] sm:w-full mx-auto">
                    <div className="flex items-center justify-between mb-6">
                        <h1 className="text-4xl font-bold text-green-400 font-mono">
                            Generated Task
                        </h1>
                    </div>
                    <AIGeneratedForm submitLabel="Generate" idTaskProgramming={Number(taskId)} onClose={handleMessageSaccess}
                        onUpdate={setTask} updateLoading={setLoading}>
                    </AIGeneratedForm>

                    {task && (
                        <div className="mt-4">
                            <Link to={`/task/info-task/${task.idTaskProgramming}`} title="View Task" >
                                <Button className="w-full md:w-fit btn-animation">Open</Button>
                            </Link>
                            <TaskProgrammingCard task={task} isEditRole={isEditRole(user?.roles ?? []) ?? user?.id === task.idPlayer} />
                        </div>
                    )}
                </div>
            </div>
      </>
  );
}

export default AIGenerateTask;