import { useAsyncTask } from "../useAsyncTask";
import { useCallback } from "react";
import { fetchSelectRoles } from "@/services/player";

export function useSelectRoles() {
    const { run: select, loading, error } = useAsyncTask(fetchSelectRoles);

    const selectRoles = useCallback(async (
        idPlayer?: string, roles?: string []
    ): Promise<boolean> => {
        const data = await select(idPlayer, roles);
        return data ?? false;
    }, [select])

    return { selectRoles, loading, error };
}
