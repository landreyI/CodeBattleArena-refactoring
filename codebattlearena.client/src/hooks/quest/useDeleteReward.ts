import { useAsyncTask } from "../useAsyncTask";
import { useCallback } from "react";
import { fetchDeleteReward } from "@/services/quest";
import { StandardError } from "@/untils/errorHandler";

export function useDeleteReward() {

    const { run, loading, error, setError } = useAsyncTask(fetchDeleteReward);

    const deleteReward = useCallback(async (rewardId?: number): Promise<boolean> => {
        if (!rewardId) {
            setError(new StandardError("Reward ID not specified"));
            return false;
        }
        return (await run(rewardId)) ?? false;
    }, [run]);

    return { deleteReward, error, loading };
}