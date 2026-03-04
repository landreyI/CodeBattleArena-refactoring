import { useCallback, useEffect, useState } from "react";
import { fetchGetBestResult } from "@/services/session";
import { PlayerSession } from "@/models/dbModels";
import { StandardError } from "@/untils/errorHandler";
import { useAsyncTask } from "../useAsyncTask";

export function useBestResult(sessionId: string | undefined) {
    const [playerSession, setPlayerSession] = useState<PlayerSession | null>(null);
    const { run: bestResult, loading, error, setError } = useAsyncTask(fetchGetBestResult);

    const loadBestResult = useCallback(async () => {
        if (!sessionId) return;
        const data = await bestResult(sessionId);
        if (data) {
            setPlayerSession(data);
        } 

    }, [bestResult, sessionId])

    useEffect(() => {
        if (!sessionId) {
            setError(new StandardError("Session ID not specified"));
            return;
        }

        loadBestResult();

    }, [loadBestResult]);

    return { playerSession, loading, error, reloadBestResult: loadBestResult };
}