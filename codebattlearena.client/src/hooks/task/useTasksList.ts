import { useEffect, useState } from "react";
import { TaskProgramming } from "@/models/dbModels";
import { useCallback } from "react";
import { fetchGetTasks } from "@/services/task";
import { TaskProgrammingFilters } from "@/models/filters";
import { useAsyncTask } from "../useAsyncTask";

export function useTasksList(filter?: TaskProgrammingFilters) {
    const [tasks, setTasks] = useState<TaskProgramming[]>([]);
    const { run: load, loading, error } = useAsyncTask(fetchGetTasks);

    const loadTasks = useCallback(async () => {
        try {
            const data = await load(filter);
            setTasks(data ?? []);
        } catch {
            setTasks([]);
        }
    }, [filter, load]);

    useEffect(() => {
        loadTasks();
    }, [loadTasks]);

    return { tasks, setTasks, loading, error, reloadTasks: loadTasks };
}