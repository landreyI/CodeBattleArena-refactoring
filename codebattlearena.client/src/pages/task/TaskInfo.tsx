import { href, useNavigate, useParams } from "react-router-dom";
import { useTask } from "@/hooks/task/useTask";
import EmptyState from "@/components/common/EmptyState";
import ErrorMessage from "@/components/common/ErrorMessage";
import LoadingScreen from "@/components/common/LoadingScreen";
import TaskProgrammingCard from "@/components/cards/TaskProgrammingCard";
import InlineNotification from "@/components/common/InlineNotification";
import { useState } from "react";
import SettingMenu from "@/components/menu/SettingMenu";
import { TaskProgramming } from "@/models/dbModels";
import { useDeleteTask } from "@/hooks/task/useDeleteTask";
import { useAuth } from "@/contexts/AuthContext";
import { isEditRole } from "@/untils/businessRules";
import { Button } from "@/components/ui/button";
import { useActiveSession } from "@/contexts/ActiveSessionContext";
import { useSelectTaskForSession } from "@/hooks/session/useSelectTaskForSession";
import { useTaskEventsHub } from "@/hooks/hubs/task/useTaskEventsHub";
import EditModal from "@/components/modals/EditModal";
import TaskForm from "@/components/forms/TaskForm";
import { MenuAction } from "@/components/menu/GenericDropdownMenu";

export function TaskInfo() {
    const { taskId } = useParams<{ taskId: string }>();
    const { task, setTask, loading: taskLoad, error: taskError } = useTask(Number(taskId));
    const [notification, setNotification] = useState<string | null>(null);
    const { deleteTask, error: deleteError } = useDeleteTask();
    const { selectTask, error: selectTaskError } = useSelectTaskForSession();
    const { user } = useAuth();
    const { activeSession } = useActiveSession();
    const navigate = useNavigate();
    const [showEditTask, setShowEditTask] = useState(false);

    useTaskEventsHub(Number(taskId), {
        onDelete: () => navigate("/home"),
        onUpdate: (updatedTask) => {
            setTask(updatedTask);
        },
    });

    const handleUpdateTask = (updatedTask: TaskProgramming) => {
        setTask(updatedTask);
    };

    const handleDeletTask = async () => {
        const isSuccess = await deleteTask(Number(taskId));
    };

    const handleSelectForSession = async () => {
        setNotification(null);
        const isSuccess = await selectTask(activeSession?.idSession ?? null, Number(taskId));
        if (isSuccess)
            navigate(`/session/info-session/${activeSession?.idSession}`);
    };

    const actions: MenuAction[] = [
        ...(task?.idTaskProgramming
            ? [
                {
                    label: "Change AI",
                    shortcut: "⌘AI",
                    href: `/task/ai-generate-task/${task?.idTaskProgramming}`,
                },
            ]
            : []),
    ];


    if (taskLoad) return <LoadingScreen />
    if (taskError) return <ErrorMessage error={taskError} />;
    if (!task) return <EmptyState message="Task not found" />;

    const error = deleteError || selectTaskError;

    return (
        <>
            {error && <InlineNotification message={error.message} className="bg-red" />}

            {notification && (
                <InlineNotification message={notification} className="bg-blue" />
            )}

            <div className="glow-box">
                <div className="md:w-[60vw] sm:w-[100vw] mx-auto">
                    <div className="flex flex-col md:flex-row items-center md:justify-between gap-4 mb-6">
                        {activeSession && (
                            <Button
                                className="btn-green btn-animation"
                                onClick={handleSelectForSession}
                            >
                                Select to play
                            </Button>
                        )}
                        <h1 className="text-4xl font-bold text-primary font-mono">
                            Task - details
                        </h1>
                        {(user && isEditRole(user.roles)) && (
                            <SettingMenu
                                setShowEdit={setShowEditTask}
                                handleDelet={handleDeletTask}
                                actionsProp={actions}
                            />
                        )}
                    </div>

                    <TaskProgrammingCard task={task} isEditRole={isEditRole(user?.roles ?? [])} />
                </div>
            </div>
            {task && (
                <EditModal open={showEditTask} title="Edit Task" onClose={() => setShowEditTask(false)}>
                    <TaskForm task={task} onClose={() => setShowEditTask(false)} onUpdate={handleUpdateTask} submitLabel="Save"></TaskForm>
                </EditModal>
            )}
        </>
    );
}

export default TaskInfo;