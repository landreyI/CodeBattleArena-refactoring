import { useState, useCallback } from "react";
import { useAsyncTask } from "../useAsyncTask";
import { fetchFinishTaskPlayer } from "@/services/playerSession";

export function useFinishTaskPlayer() {
    const [isCompleted, setIsCompleted] = useState<boolean>(false);
    const { run: finish, loading, error } = useAsyncTask(fetchFinishTaskPlayer);

    const finishTask = useCallback(async () => {
        const data = await finish();
        if (data) {
            setIsCompleted(data);
        }
        return data;
    }, [finish]);

    return { isCompleted, loading, error, finishTask};
}