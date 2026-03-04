import { useEffect, useState } from "react";
import { Reward } from "@/models/dbModels";
import { useCallback } from "react";
import { fetchGetTaskPlayRewards } from "@/services/quest";
import { useAsyncTask } from "../useAsyncTask";

export function useTaskPlayRewards(idTaskPlay?: number) {
    const [taskPlayRewards, setTaskPlayRewards] = useState<Reward[]>([]);
    const { run: load, loading, error } = useAsyncTask(fetchGetTaskPlayRewards);

    const loadRewards = useCallback(async () => {
        try {
            const data = await load(idTaskPlay);
            setTaskPlayRewards(data ?? []);
        } catch {
            setTaskPlayRewards([]);
        }
    }, [load, idTaskPlay]);

    useEffect(() => {
        if(idTaskPlay)
            loadRewards()
    }, [loadRewards])

    return { taskPlayRewards, setTaskPlayRewards, loading, error, reloadTaskPlayRewards: loadRewards };
}