
import { TaskPlay } from "@/models/dbModels";
import { fetchUpdateTaskPlay } from "@/services/quest";
import { useAsyncTask } from "../useAsyncTask";
import { useCallback } from "react";

export function useUpdateTaskPlay() {
    const { run: update, loading, error } = useAsyncTask(fetchUpdateTaskPlay);

    /**
    * Update TaskPlay.
    * @param values - The form values for creating a TaskPlay.
    * @throws StandardError if the session creation fails.
    */
    const updateTaskPlay = useCallback(async (
        taskPlay: TaskPlay,
        idRewards?: number[],
    ): Promise<boolean> => {
        const data = await update(taskPlay, idRewards);
        return data ?? false;
    }, [update])

    return { updateTaskPlay, loading, error };
}
