import { useEffect, useState } from "react";
import { LeaguePlayers } from "@/models/dbModels";
import { useCallback } from "react";
import { useAsyncTask } from "../useAsyncTask";
import { fetchGetPlayersLeagues } from "@/services/league";

export function usePlayersLeagues() {
    const [playersLeagues, setPlayersLeagues] = useState<LeaguePlayers[]>([]);
    const { run: load, loading, error } = useAsyncTask(fetchGetPlayersLeagues);

    const loadPlayersLeagues = useCallback(async () => {
        try {
            const data = await load();
            setPlayersLeagues(data ?? []);
        } catch {
            setPlayersLeagues([]);
        }
    }, [load]);

    useEffect(() => {
        loadPlayersLeagues();
    }, [loadPlayersLeagues]);

    return { playersLeagues, setPlayersLeagues, loadPlayersLeagues, loading, error }
}