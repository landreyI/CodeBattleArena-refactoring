import { fetchStartGame } from "@/services/session";
import { useCallback } from "react";
import { useAsyncTask } from "../useAsyncTask";

export function useStartGame() {
    const { run: start, loading, error } = useAsyncTask(fetchStartGame);

    const startGame = useCallback(async (idSession: string | null): Promise<boolean | null> => {
        if (!idSession) return false;
        const data = await start(idSession);
        return data;
    }, [start]);

    return { loading, error, startGame };
}