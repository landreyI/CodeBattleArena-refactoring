import { useTasksList } from "@/hooks/task/useTasksList";
import TasksList from "@/components/lists/TasksList";
import EmptyState from "@/components/common/EmptyState";
import ErrorMessage from "@/components/common/ErrorMessage";
import LoadingScreen from "@/components/common/LoadingScreen";
import { useAuth } from "@/contexts/AuthContext";
import { isEditRole } from "@/untils/businessRules";
import { useDeleteTask } from "@/hooks/task/useDeleteTask";
import InlineNotification from "@/components/common/InlineNotification";
import { useLocation } from "react-router-dom";
import { useState } from "react";
import { TaskProgrammingFilters } from "@/models/filters";
import { Difficulty } from "@/models/dbModels";
import TaskFilter from "@/components/filters/TaskFilter";
import { useTaskEventsHub } from "@/hooks/hubs/task/useTaskEventsHub";
import { parseEnumParam } from "@/untils/helpers";

export function TasksListPage() {
    const location = useLocation();
    const queryParams = new URLSearchParams(location.search);

    const idLang = queryParams.get('idLang') ? Number(queryParams.get('idLang')) : undefined;
    const difficulty = queryParams.get('difficulty') ? parseEnumParam(queryParams.get('difficulty'), Difficulty, Difficulty.Easy) : undefined;

    const filterReceived: TaskProgrammingFilters = {
        idLang,
        difficulty,
    };

    const [filter, setFilter] = useState<TaskProgrammingFilters>(filterReceived);

    const { tasks, setTasks, loading: tasksLoad, error: tasksError, reloadTasks } = useTasksList(filter);
    const { deleteTask, error: deleteError } = useDeleteTask();
    const { user } = useAuth();

    useTaskEventsHub(undefined, {
        onListUpdate: (updatedTask) => {
            setTasks((prevTasks) =>
                prevTasks.map((task) =>
                    task.idTaskProgramming === updatedTask.idTaskProgramming ? updatedTask : task
                )
            );
        },
        onAdding: (addTask) => setTasks((prevTasks) => [addTask, ...prevTasks]),
        onListDelete: (taskId) => setTasks((prevTasks) => prevTasks.filter((task) => task.idTaskProgramming !== taskId)),
    });

    const handleDeletTask = async (taskId: number) => {
        const success = await deleteTask(taskId);
        if (success)
            setTasks((prevTasks) => prevTasks.filter((task) => task.idTaskProgramming !== taskId));
    };

    const handleChangeFilter = (filter: TaskProgrammingFilters) => {
        setFilter(filter);
    }

    if (tasksLoad) return <LoadingScreen />
    if (tasksError) return <ErrorMessage error={tasksError} />;

    return (
        <>
            {deleteError && <InlineNotification message={deleteError.message} className="bg-red" />}

            <TaskFilter filter={filter} onChange={handleChangeFilter} handleSearch={reloadTasks}></TaskFilter>

            {!tasks || tasks.length === 0 && (<EmptyState message="Tasks not found" />)}

            <div className="mb-5"></div>

            <TasksList
                tasks={tasks}
                cardWrapperClassName="hover:scale-[1.02] transition"
                onDelete={(user && isEditRole(user.roles)) ? handleDeletTask : undefined}
            />
        </>
    )
}

export default TasksListPage;