import { useAsyncTask } from "../useAsyncTask";
import { useCallback } from "react";
import { fetcAddFriend } from "@/services/friend";

export function useAddFriend() {
    const { run: add, loading, error } = useAsyncTask(fetcAddFriend);

    const addFriend = useCallback(async (addresseeId?: string) => {
        const data = await add(addresseeId);
        return data;
    }, [add]);

    return { addFriend, loading, error };
}
