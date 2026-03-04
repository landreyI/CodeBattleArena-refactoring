import { TaskProgramming } from "@/models/dbModels";
import { fetchCreateTask } from "@/services/task";
import { useAsyncTask } from "../useAsyncTask";
import { useCallback } from "react";

export function useCreateTask() {
    const { run: create, loading, error } = useAsyncTask(fetchCreateTask);
    /**
    * Creates a new task and returns its ID.
    * @param values - The form values for creating a task.
    * @returns The ID of the created task.
    * @throws StandardError if the session creation fails.
    */
    const createTask = useCallback(async (
        task: TaskProgramming
    ) => {
        const idTask = await create(task);
        return idTask;
    }, [create])

    return { createTask, loading, error };
}
