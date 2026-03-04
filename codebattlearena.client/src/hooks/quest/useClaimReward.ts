
import { fetchClaimReward } from "@/services/quest";
import { useAsyncTask } from "../useAsyncTask";
import { useCallback } from "react";

export function useClaimReward() {
    const { run: claim, loading, error } = useAsyncTask(fetchClaimReward);

    const claimReward = useCallback(async (idPlayer?: string, idTaskPlay?: number): Promise<boolean> => {
        const data = await claim(idPlayer, idTaskPlay);
        return data ?? false;
    }, [claim])

    return { claimReward, loading, error };
}
