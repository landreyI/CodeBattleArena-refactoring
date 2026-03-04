
import { TaskProgramming } from "@/models/dbModels";
import { fetchUpdateTask } from "@/services/task";
import { useAsyncTask } from "../useAsyncTask";
import { useCallback } from "react";

export function useUpdateTask() {
    const { run: update, loading, error } = useAsyncTask(fetchUpdateTask);

    /**
    * Update task.
    * @param values - The form values for creating a task.
    * @throws StandardError if the session creation fails.
    */
    const updateTask = useCallback(async (
        task: TaskProgramming,
    ): Promise<boolean> => {
        const data = await update(task);
        return data ?? false;
    }, [update])

    return { updateTask, loading, error };
}
