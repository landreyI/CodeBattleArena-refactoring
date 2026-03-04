import { useAsyncTask } from "../useAsyncTask";
import { useCallback } from "react";
import { fetchDeleteTaskPlay } from "@/services/quest";
import { StandardError } from "@/untils/errorHandler";

export function useDeleteTaskPlay() {

    const { run, loading, error, setError } = useAsyncTask(fetchDeleteTaskPlay);

    const deleteTaskPlay = useCallback(async (taskPlayId?: number): Promise<boolean> => {
        if (!taskPlayId) {
            setError(new StandardError("Quest ID not specified"));
            return false;
        }
        return (await run(taskPlayId)) ?? false;
    }, [run]);

    return { deleteTaskPlay, error, loading };
}