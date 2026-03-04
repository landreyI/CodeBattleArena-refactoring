import { useEffect, useState } from "react";
import { Session } from "@/models/dbModels";
import { fetchGetPlayerSessionHistory } from "@/services/player";
import { useCallback } from "react";
import { useAsyncTask } from "../useAsyncTask";

export function usePlayerSessions(playerId?: string, enabled: boolean = true) {
    const [sessions, setSessions] = useState<Session[]>([]);
    const { run: load, loading, error } = useAsyncTask(fetchGetPlayerSessionHistory);

    const loadSessions = useCallback(async () => {
        if (!playerId || !enabled) return;
        try {
            const data = await load(playerId);
            setSessions(data ?? []);
        } catch {
            setSessions([]);
        }

    }, [load, playerId]);

    useEffect(() => {
        loadSessions();

    }, [enabled, loadSessions]);

    return { sessions, setSessions, loading, error }
}