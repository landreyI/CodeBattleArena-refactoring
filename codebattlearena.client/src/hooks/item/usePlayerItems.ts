import { useEffect, useState, useCallback } from "react";
import { Item, TypeItem } from "@/models/dbModels";
import { useAsyncTask } from "../useAsyncTask";
import { fetchGetPlayerItems } from "@/services/item";
export function usePlayerItems(idPlayer?: string, typeItem?: TypeItem) {
    const [playerItems, setPlayerItems] = useState<Item[]>([]);
    const { run: load, loading, error } = useAsyncTask(fetchGetPlayerItems);

    const loadPlayerItems = useCallback(async () => {
        if (!idPlayer) return;
        try {
            const data = await load(idPlayer, typeItem);
            setPlayerItems(data ?? []);
        } catch {
            setPlayerItems([]);
        }
    }, [load, idPlayer, typeItem]);

    useEffect(() => {
        loadPlayerItems();
    }, [loadPlayerItems]);

    return { playerItems, setPlayerItems, loading, error, reloadPlayerItems: loadPlayerItems };
}