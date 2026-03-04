
import { Reward } from "@/models/dbModels";
import { fetchUpdateReward } from "@/services/quest";
import { useAsyncTask } from "../useAsyncTask";
import { useCallback } from "react";

export function useUpdateReward() {
    const { run: update, loading, error } = useAsyncTask(fetchUpdateReward);

    /**
    * Update reward.
    * @param values - The form values for creating a reward.
    * @throws StandardError if the session creation fails.
    */
    const updateReward = useCallback(async (
        quest: Reward,
    ): Promise<boolean> => {
        const data = await update(quest);
        return data ?? false;
    }, [update])

    return { updateReward, loading, error };
}
