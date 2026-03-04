import { useAsyncTask } from "../useAsyncTask";
import { useCallback } from "react";
import { fetchChangeActiveItem } from "@/services/player";

export function useChangeActiveItem() {
    const { run: change, loading, error } = useAsyncTask(fetchChangeActiveItem);

    const changeActiveItem = useCallback(async (
        idPlayer?: string,
        idItem?: number
    ): Promise<boolean> => {
        const data = await change(idPlayer, idItem);
        return data ?? false;
    }, [change]);

    return { changeActiveItem, loading, error };
}
