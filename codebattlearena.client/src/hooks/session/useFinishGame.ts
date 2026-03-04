import { fetchFinishGame } from "@/services/session";
import { useCallback } from "react";
import { useAsyncTask } from "../useAsyncTask";

export function useFinishGame() {
    const { run: start, loading, error } = useAsyncTask(fetchFinishGame);

    const finishGame = useCallback(async (idSession: string | null): Promise<boolean | null> => {
        if (!idSession) return false;
        const data = await start(idSession);
        return data;
    }, [start]);

    return { loading, error, finishGame };
}