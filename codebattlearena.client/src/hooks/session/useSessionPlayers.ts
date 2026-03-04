import { useEffect, useState, useCallback } from "react";
import { Player } from "@/models/dbModels";
import { fetchGetSessionPlayers } from "@/services/session";
import { useAsyncTask } from "../useAsyncTask";

export function useSessionPlayers(sessionId?: string, enabled: boolean = true) {
    const [players, setPlayers] = useState<Player[]>([]);
    const { run: load, loading, error } = useAsyncTask(fetchGetSessionPlayers);

    const loadPlayers = useCallback(async () => {
        if (!sessionId) return;
        try {
            const data = await load(sessionId);
            setPlayers(data ?? []);
        } catch {
            setPlayers([]);
        }
    }, [load, sessionId]);

    useEffect(() => {
        if (!enabled) return;
        loadPlayers();
    }, [enabled, loadPlayers]);

    return { players, setPlayers, loading, error, reloadPlayers: loadPlayers };
}
