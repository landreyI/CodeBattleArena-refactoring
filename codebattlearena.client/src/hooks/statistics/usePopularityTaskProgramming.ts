import { useCallback, useEffect, useState } from "react";
import { PopularityTask } from "@/models/statistics";
import { useAsyncTask } from "../useAsyncTask";
import { fetchPopularityTaskProgramming } from "@/services/statistics";

export function usePopularityTaskProgramming() {
    const [statistics, setStatistics] = useState<PopularityTask[]>([]);
    const { run: load, loading, error } = useAsyncTask(fetchPopularityTaskProgramming);

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