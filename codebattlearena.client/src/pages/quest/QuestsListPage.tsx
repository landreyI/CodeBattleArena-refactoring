import { useTasksPlays } from "@/hooks/quest/useTasksPlays";
import { useListPlayerProgress } from "@/hooks/quest/useListPlayerProgress";
import { useAuth } from "@/contexts/AuthContext";
import EmptyState from "@/components/common/EmptyState";
import QuestsList from "@/components/lists/QuestsList";
import LoadingScreen from "@/components/common/LoadingScreen";
import ErrorMessage from "@/components/common/ErrorMessage";

export function QuestsListPage() {
    const { user } = useAuth();
    const { tasksPlayes, setTasksPlayes, loading: tasksPlaysLoad,
        error: tasksPlaysError, reloadTasksPlays } = useTasksPlays();

    const { listPlayerProgress, setListPlayerProgress, loading: listProgressLoad,
        error: listProgressError, reloadListPlayerProgress } = useListPlayerProgress(user?.id);

    if (tasksPlaysLoad) return <LoadingScreen />
    if (tasksPlaysError) return <ErrorMessage error={tasksPlaysError} />;

    return (
        <>
            {(!tasksPlayes || tasksPlayes.length === 0) && (<EmptyState message="Quests not found" />)}

            <QuestsList
                tasksPlays={tasksPlayes}
                playerTasksPlays={listPlayerProgress}
                cardWrapperClassName="hover:scale-[1.02] transition"
            />
        </>
    );
}

export default QuestsListPage;