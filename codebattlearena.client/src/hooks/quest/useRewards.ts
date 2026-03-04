import { useEffect, useState } from "react";
import { Reward } from "@/models/dbModels";
import { useCallback } from "react";
import { fetchGetRewards } from "@/services/quest";
import { useAsyncTask } from "../useAsyncTask";

export function useRewards() {
    const [rewards, setRewards] = useState<Reward[]>([]);
    const { run: load, loading, error } = useAsyncTask(fetchGetRewards);

    const loadRewards = useCallback(async () => {
        try {
            const data = await load();
            setRewards(data ?? []);
        } catch {
            setRewards([]);
        }
    }, [load]);

    useEffect(() => {
        loadRewards();
    }, [loadRewards])

    return { rewards, setRewards, loading, error, reloadRewards: loadRewards };
}