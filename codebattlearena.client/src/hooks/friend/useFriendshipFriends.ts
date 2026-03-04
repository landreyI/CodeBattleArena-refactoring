import { useState, useCallback } from "react";
import { Friend } from "@/models/dbModels";
import { useAsyncTask } from "../useAsyncTask";
import { fetchGetFriendshipFriends } from "@/services/friend";
export function useFriendshipFriends() {
    const [friendships, setFriendships] = useState<Friend[]>([]);
    const { run: load, loading, error } = useAsyncTask(fetchGetFriendshipFriends);

    const loadFriendships = useCallback(async () => {
        try {
            const data = await load();
            setFriendships(data ?? []);
        } catch {
            setFriendships([]);
        }
    }, [load]);

    return { friendships, setFriendships, loading, error, reloadFriendships: loadFriendships };
}