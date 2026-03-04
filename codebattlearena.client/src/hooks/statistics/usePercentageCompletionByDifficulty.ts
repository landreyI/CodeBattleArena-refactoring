import { useCallback, useEffect, useState } from "react";
import { PercentageCompletion } from "@/models/statistics";
import { useAsyncTask } from "../useAsyncTask";
import { fetchPercentageCompletionByDifficulty } from "@/services/statistics";

export function usePercentageCompletionByDifficulty() {
    const [statistics, setStatistics] = useState<PercentageCompletion[]>([]);
    const { run: load, loading, error } = useAsyncTask(fetchPercentageCompletionByDifficulty);

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