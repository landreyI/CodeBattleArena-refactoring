import { useEffect, useState } from "react";
import { fetchGetPlayer } from "@/services/player";
import { Player } from "@/models/dbModels";
import { StandardError } from "@/untils/errorHandler";
import { useAsyncTask } from "../useAsyncTask";

export function usePlayer(playerId: string | undefined) {
    const [player, setPlayer] = useState<Player | null>(null);
    const [isEdit, setIsEdit] = useState<boolean>();
    const { run: loadPlayer, loading, error, setError } = useAsyncTask(fetchGetPlayer);

    useEffect(() => {
        if (!playerId) {
            setError(new StandardError("Player ID not specified"));
            return;
        }

        (async () => {
            const data = await loadPlayer(playerId);
            if (data) {
                setPlayer(data.player);
                setIsEdit(data.isEdit);
            }
        })();


    }, [playerId, loadPlayer]);

    return { player, setPlayer, isEdit, loading, error };
}