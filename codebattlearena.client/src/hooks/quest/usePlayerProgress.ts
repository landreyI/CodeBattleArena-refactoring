import { useEffect, useState } from "react";
import { PlayerTaskPlay } from "@/models/dbModels";
import { useCallback } from "react";
import { fetchGetPlayerProgress } from "@/services/quest";
import { useAsyncTask } from "../useAsyncTask";
import { StandardError } from "@/untils/errorHandler";

export function usePlayerProgress(idPlayer?: string, idTaskPlay?: number) {
    const [playerProgress, setPlayerProgress] = useState<PlayerTaskPlay>();
    const { run: load, loading, error, setError } = useAsyncTask(fetchGetPlayerProgress);

    const loadPlayerProgress = useCallback(async () => {
        if (!idPlayer || !idTaskPlay) {
            setError(new StandardError("ID not specified"));
            return;
        }

        const data = await load(idPlayer, idTaskPlay);
        if (data) {
            setPlayerProgress(data);
        }
    }, [load, idPlayer, idTaskPlay, setError]);

    useEffect(() => {
        loadPlayerProgress();
    }, [loadPlayerProgress])

    return { playerProgress, setPlayerProgress, loading, error, reloadProgress: loadPlayerProgress };
}