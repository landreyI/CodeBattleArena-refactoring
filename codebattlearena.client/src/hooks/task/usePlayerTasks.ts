import { useEffect, useState, useCallback } from "react";
import { TaskProgramming } from "@/models/dbModels";
import { useAsyncTask } from "../useAsyncTask";
import { fetchGetPlayerTasks } from "@/services/task";
export function usePlayerTasks(idPlayer: string) {
    const [playerTasks, setPlayerTasks] = useState<TaskProgramming[]>([]);
    const { run: load, loading, error } = useAsyncTask(fetchGetPlayerTasks);

    const loadPlayerTasks = useCallback(async () => {
        if (!idPlayer) return;
        try {
            const data = await load(idPlayer);
            setPlayerTasks(data ?? []);
        } catch {
            setPlayerTasks([]);
        }
    }, [load, idPlayer]);

    useEffect(() => {
        loadPlayerTasks();
    }, [loadPlayerTasks]);

    return { playerTasks, setPlayerTasks, loading, error, reloadPlayerTasks: loadPlayerTasks };
}