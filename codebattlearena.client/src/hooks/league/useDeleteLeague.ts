import { useCallback } from "react";
import { fetchDeleteLeague } from "@/services/league";
import { useAsyncTask } from "../useAsyncTask";
import { StandardError } from "@/untils/errorHandler";

export function useDeleteLeague() {
    const { run, loading, error, setError } = useAsyncTask(fetchDeleteLeague);

    const deleteLeague = useCallback(async (leagueId?: number): Promise<boolean> => {
        if (!leagueId) {
            setError(new StandardError("League ID not specified"));
            return false;
        }
        return (await run(leagueId)) ?? false;
    }, [run]);

    return { deleteLeague, error, loading };
}