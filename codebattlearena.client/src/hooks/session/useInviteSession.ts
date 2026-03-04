import { fetchInviteSession } from "@/services/session";
import { useAsyncTask } from "../useAsyncTask";
import { useCallback } from "react";
import { StandardError } from "@/untils/errorHandler";

export function useInviteSession() {

    const { run, loading, error, setError } = useAsyncTask(fetchInviteSession);


    const inviteSession = useCallback(async (sessionId: string | null, idPlayersInvite?: string[]): Promise<boolean> => {
        if (!idPlayersInvite || !sessionId) {
            setError(new StandardError("ID not specified"));
            return false;
        }
        return (await run(sessionId, idPlayersInvite)) ?? false;
    }, [run]);

    return { inviteSession, error, loading };
}