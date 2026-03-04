import EmptyState from "@/components/common/EmptyState";
import ErrorMessage from "@/components/common/ErrorMessage";
import LoadingScreen from "@/components/common/LoadingScreen";
import InlineNotification from "@/components/common/InlineNotification";
import { useAuth } from "@/contexts/AuthContext";
import { useParams } from "react-router-dom";
import { useDeleteTask } from "@/hooks/task/useDeleteTask";
import TasksList from "@/components/lists/TasksList";
import { usePlayerTasks } from "@/hooks/task/usePlayerTasks";

export function PlayerTasksPage() {
    const { playerId } = useParams<{ playerId: string }>();

    const { playerTasks, setPlayerTasks, loading: palyerTasksLoad, error: palyerTasksError, reloadPlayerTasks } = usePlayerTasks(playerId ?? "");
    const { deleteTask, error: deleteError } = useDeleteTask();
    const { user } = useAuth();

    const handleDeletTask = async (taskId: number) => {
        const success = await deleteTask(taskId);
        if (success)
            setPlayerTasks((prevTasks) => prevTasks.filter((task) => task.idTaskProgramming !== taskId));
    };

    if (palyerTasksLoad) return <LoadingScreen />
    if (palyerTasksError) return <ErrorMessage error={palyerTasksError} />;

    const error = deleteError;

    return (
        <>
            {error && <InlineNotification message={error.message} className="bg-red" />}

            {!playerTasks || playerTasks.length === 0 && (<EmptyState message="Tasks not found" />)}

            <div className="mb-5"></div>

            <TasksList
                tasks={playerTasks}
                cardWrapperClassName="hover:scale-[1.02] transition"
                onDelete={(user && user.id === playerId) ? handleDeletTask : undefined}
            />
        </>
    )
}

export default PlayerTasksPage;