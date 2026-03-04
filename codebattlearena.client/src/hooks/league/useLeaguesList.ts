import { useEffect, useState } from "react";
import { League } from "@/models/dbModels";
import { useCallback } from "react";
import { useAsyncTask } from "../useAsyncTask";
import { fetchGetListLeagues } from "@/services/league";

export function useLeaguesList() {
    const [leagues, setLeagues] = useState<League[]>([]);
    const { run: load, loading, error } = useAsyncTask(fetchGetListLeagues);

    const loadLeagues = useCallback(async () => {
        try {
            const data = await load();
            setLeagues(data ?? []);
        } catch {
            setLeagues([]);
        }
    }, [load]);

    useEffect(() => {
        loadLeagues();
    }, [loadLeagues]);

    return { leagues, setLeagues, loadLeagues, loading, error }
}