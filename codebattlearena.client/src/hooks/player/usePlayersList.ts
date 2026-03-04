import { useEffect, useState } from "react";
import { Player } from "@/models/dbModels";
import { fetchGetPlayersList } from "@/services/player";
import { useCallback } from "react";
import { useAsyncTask } from "../useAsyncTask";
import { PlayerFilters } from "@/models/filters";

export function usePlayersList(filter?: PlayerFilters) {
    const [players, setPlayers] = useState<Player[]>([]);
    const { run: load, loading, error } = useAsyncTask(fetchGetPlayersList);

    const loadPlayers = useCallback(async () => {
        try {
            const data = await load(filter);
            setPlayers(data ?? []);
        } catch {
            setPlayers([]);
        }
    }, [load, filter]);

    useEffect(() => {
        loadPlayers();
    }, [loadPlayers]);

    return { players, setPlayers, loadPlayers, loading, error }
}