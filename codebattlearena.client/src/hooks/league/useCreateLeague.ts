import { League } from "@/models/dbModels";
import { fetchCreateLeague } from "@/services/league";
import { useAsyncTask } from "../useAsyncTask";
import { useCallback } from "react";

export function useCreateLeague() {
    const { run: create, loading, error } = useAsyncTask(fetchCreateLeague);
    /**
    * Creates a new league.
    * @param values - The form values for creating a league.
    * @throws StandardError if the league creation fails.
    */
    const createLeague = useCallback(async (league: League) => {
        const data = await create(league);
        return data;
    }, [create]);

    return { createLeague, loading, error };
}
