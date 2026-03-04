import { useEffect, useState } from "react";
import { PlayerTaskPlay } from "@/models/dbModels";
import { useCallback } from "react";
import { fetchGetListPlayerProgress } from "@/services/quest";
import { useAsyncTask } from "../useAsyncTask";

export function useListPlayerProgress(idPlayer?: string) {
    const [listPlayerProgress, setListPlayerProgress] = useState<PlayerTaskPlay[]>([]);
    const { run: load, loading, error } = useAsyncTask(fetchGetListPlayerProgress);

    const loadListPlayerProgress = useCallback(async () => {
        try {
            const data = await load(idPlayer);
            setListPlayerProgress(data ?? []);
        } catch {
            setListPlayerProgress([]);
        }
    }, [load, idPlayer]);

    useEffect(() => {
        if (idPlayer) {
            loadListPlayerProgress();
        }
    }, [loadListPlayerProgress])

    return { listPlayerProgress, setListPlayerProgress, loading, error, reloadListPlayerProgress: loadListPlayerProgress };
}