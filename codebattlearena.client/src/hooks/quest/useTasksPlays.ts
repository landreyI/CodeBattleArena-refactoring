import { useEffect, useState } from "react";
import { TaskPlay } from "@/models/dbModels";
import { useCallback } from "react";
import { fetchGetQuests } from "@/services/quest";
import { useAsyncTask } from "../useAsyncTask";

export function useTasksPlays() {
    const [tasksPlayes, setTasksPlayes] = useState<TaskPlay[]>([]);
    const { run: load, loading, error } = useAsyncTask(fetchGetQuests);

    const loadTasksPlays = useCallback(async () => {
        try {
            const data = await load();
            setTasksPlayes(data ?? []);
        } catch {
            setTasksPlayes([]);
        }
    }, [load]);

    useEffect(() => {
        loadTasksPlays();
    }, [loadTasksPlays])

    return { tasksPlayes, setTasksPlayes, loading, error, reloadTasksPlays: loadTasksPlays };
}