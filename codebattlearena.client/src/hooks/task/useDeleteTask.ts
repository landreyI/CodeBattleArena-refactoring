import { fetchDeleteTask } from "@/services/task";
import { useAsyncTask } from "../useAsyncTask";
import { useCallback } from "react";
import { StandardError } from "@/untils/errorHandler";

export function useDeleteTask() {
    const { run, loading, error, setError } = useAsyncTask(fetchDeleteTask);

    const deleteTask = useCallback(async (taskId?: number): Promise<boolean> => {
        if (!taskId) {
            setError(new StandardError("TaskProgramming ID not specified"));
            return false;
        }
        const data = await run(taskId);
        return data ?? false;
    }, [run]);

    return { deleteTask, error, loading };
}