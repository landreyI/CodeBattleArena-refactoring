import { useState, useCallback } from "react";
import { fetchJoinSession } from "@/services/session";
import { useAsyncTask } from "../useAsyncTask";

export function useSessionJoin() {
    const [isCompleted, setIsCompleted] = useState<boolean>(false);

    const { run: join, loading, error } = useAsyncTask(fetchJoinSession);

    const joinSession = useCallback(async (idSession: string, password?: string): Promise<boolean | null> => {
        const data = await join(idSession, { password });

        if (data) {
            setIsCompleted(true);
        }

        return data;
    }, [join]);

    return { isCompleted, loading, error, joinSession };
}