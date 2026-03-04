import { useState, useEffect } from "react";
import { PlayerSession } from "@/models/dbModels";
import { fetchGetPlayerSessionInfo } from "@/services/session";
import { useCallback } from "react";
import { useAsyncTask } from "../useAsyncTask";
import { StandardError } from "@/untils/errorHandler";

export function usePlayerSessionInfo(playerId?: string, sessionId?: string) {
    const [playerSession, setPlayerSession] = useState<PlayerSession | null>(null);
    const { run: load, loading, error, setError } = useAsyncTask(fetchGetPlayerSessionInfo);

    const loadPlayerSessionInfo = useCallback(async () => {
        if (!sessionId || !playerId) {
            setError(new StandardError("ID not specified"));
            return;
        }
        try {
            const data = await load(sessionId, playerId);
            setPlayerSession(data ?? null);
        } catch {
            setPlayerSession(null);
        }

    }, [load, playerId, sessionId]);

    useEffect(() => {
        loadPlayerSessionInfo();
    }, [loadPlayerSessionInfo])

    return { playerSession, setPlayerSession, loadPlayerSessionInfo, loading, error }
}