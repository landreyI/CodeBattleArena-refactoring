import { useEffect, useState, useCallback } from "react";
import { Friend } from "@/models/dbModels";
import { useAsyncTask } from "../useAsyncTask";
import { fetchGetListFriends } from "@/services/friend";
export function useFriends() {
    const [friends, setFriends] = useState<Friend[]>([]);
    const { run: load, loading, error } = useAsyncTask(fetchGetListFriends);

    const loadFriends = useCallback(async () => {
        try {
            const data = await load();
            setFriends(data ?? []);
        } catch {
            setFriends([]);
        }
    }, [load]);

    useEffect(() => {
        loadFriends();
    }, [loadFriends]);

    return { friends, setFriends, loading, error, reloadFriends: loadFriends };
}