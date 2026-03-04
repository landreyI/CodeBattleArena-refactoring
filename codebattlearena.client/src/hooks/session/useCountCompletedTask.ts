import { useCallback, useState, useEffect } from "react";
import { useAsyncTask } from "../useAsyncTask";
import { fetchGetFinishedPlayersCount, CompletionCount } from "@/services/session";

export function useCountCompletedTask(idSession?: string) {
    const [count, setCount] = useState<CompletionCount>();
    const { run: load, loading, error } = useAsyncTask(fetchGetFinishedPlayersCount);

    const loadCount = useCallback(async () => {
        if (!idSession) return;

        const data = await load(idSession);
        if (data !== undefined && data !== null) {
            setCount(data);
        }

    }, [load, idSession])

    useEffect(() => {
        if (!idSession) return;
        loadCount();
    }, [loadCount])

    return { count, setCount, loading, error, reloadCountCompleted: loadCount }
}