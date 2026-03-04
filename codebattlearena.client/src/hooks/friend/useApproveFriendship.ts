import { useAsyncTask } from "../useAsyncTask";
import { useCallback } from "react";
import { fetchApproveFriendship } from "@/services/friend";

export function useApproveFriendship() {
    const { run: approve, loading, error } = useAsyncTask(fetchApproveFriendship);

    const approveFriendship = useCallback(async (
        idFriend?: number,
    ): Promise<boolean> => {
        const data = await approve(idFriend);
        return data ?? false;
    }, [approve]);

    return { approveFriendship, loading, error };
}
