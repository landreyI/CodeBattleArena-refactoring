import { Reward, TaskPlay } from "@/models/dbModels";
import { fetchCreateTaskPlay } from "@/services/quest";
import { useAsyncTask } from "../useAsyncTask";
import { useCallback } from "react";

export function useCreateTaskPlay() {
    const { run: create, loading, error } = useAsyncTask(fetchCreateTaskPlay);
    /**
    * Creates a new TaskPlay and returns its ID.
    * @param values - The form values for creating a TaskPlay.
    * @returns The ID of the created TaskPlay.
    * @throws StandardError if the session creation fails.
    */
    const createTaskPlay = useCallback(async (
        taskPlay: TaskPlay,
        idRewards?: number[]
    ) => {
        const idTaskPlay = await create(taskPlay, idRewards);
        return idTaskPlay;
    }, [create])

    return { createTaskPlay, loading, error };
}
