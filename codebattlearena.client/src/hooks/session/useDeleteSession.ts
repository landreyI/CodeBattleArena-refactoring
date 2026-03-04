import { fetchDeleteSession } from "@/services/session";
import { useAsyncTask } from "../useAsyncTask";
import { useCallback } from "react";
import { StandardError } from "@/untils/errorHandler";

export function useDeleteSession() {

    const { run, loading, error, setError } = useAsyncTask(fetchDeleteSession);


    const deleteSession = useCallback(async (sessionId?: string): Promise<boolean> => {
        if (!sessionId) {
            setError(new StandardError("Session ID not specified"));
            return false;
        }
        return (await run(sessionId)) ?? false;
    }, [run]);

    return { deleteSession, error, loading };
}