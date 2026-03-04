import { useCallback, useEffect, useState } from "react";
import { AvgCompletionTime } from "@/models/statistics";
import { useAsyncTask } from "../useAsyncTask";
import { fetchAvgTaskCompletionTimeByDifficulty } from "@/services/statistics";

export function useAvgTaskCompletionTimeByDifficulty() {
    const [statistics, setStatistics] = useState<AvgCompletionTime[]>([]);
    const { run: load, loading, error } = useAsyncTask(fetchAvgTaskCompletionTimeByDifficulty);

    const loadStatistics = useCallback(async () => {
        try {
            const data = await load();
            setStatistics(data ?? []);
        } catch {
            setStatistics([]);
        }
    }, [load]);

    useEffect(() => {
        loadStatistics();
    }, [loadStatistics]);

    return { statistics, setStatistics, loading, error, reloadStatistics: loadStatistics };
}