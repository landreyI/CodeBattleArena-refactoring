import { useState } from "react";
import { fetchCheckCodePlayer } from "@/services/playerSession";
import { useCallback } from "react";
import { useAsyncTask } from "../useAsyncTask";

export function useCheckPlayerCode() {
    const { run: check, loading, error } = useAsyncTask(fetchCheckCodePlayer);

    const checkCode = useCallback(async (code: string) => {
        const data = await check(code);
        return data;
    }, [check]);

    return { loading, error, checkCode };
}