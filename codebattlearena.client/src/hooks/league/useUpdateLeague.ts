import { League } from "@/models/dbModels";
import { fetchEditLeague } from "@/services/league";
import { useAsyncTask } from "../useAsyncTask";
import { useCallback } from "react";

export function useUpdateLeague() {
    const { run: update, loading, error } = useAsyncTask(fetchEditLeague);
    /**
    * Update league.
    * @param values - The form values for creating a league.
    * @throws StandardError if the session creation fails.
    */
    const updateLeague = useCallback(async (
        league: League,
    ): Promise<boolean> => {
        const data = await update(league);
        return data ?? false;
    }, [update]);

    return { updateLeague, loading, error };
}
