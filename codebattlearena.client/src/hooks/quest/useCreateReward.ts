import { Reward } from "@/models/dbModels";
import { fetchCreateReward } from "@/services/quest";
import { useAsyncTask } from "../useAsyncTask";
import { useCallback } from "react";

export function useCreateReward() {
    const { run: create, loading, error } = useAsyncTask(fetchCreateReward);
    /**
    * Creates a new reward and returns its ID.
    * @param values - The form values for creating a reward.
    * @returns The ID of the created reward.
    * @throws StandardError if the session creation fails.
    */
    const createReward = useCallback(async (
        reward: Reward
    ) => {
        const idReward = await create(reward);
        return idReward;
    }, [create])

    return { createReward, loading, error };
}
