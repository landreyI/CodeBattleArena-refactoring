import { useCallback, useEffect, useState } from "react";
import { LanguagePopularity } from "@/models/statistics";
import { useAsyncTask } from "../useAsyncTask";
import { fetchPopularityLanguagesProgramming } from "@/services/statistics";

export function usePopularityLanguagesProgramming() {
    const [statistics, setStatistics] = useState<LanguagePopularity[]>([]);
    const { run: load, loading, error } = useAsyncTask(fetchPopularityLanguagesProgramming);

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