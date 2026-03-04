import { useEffect, useState, useCallback } from "react";
import { ItemFilters } from "@/models/filters";
import { Item } from "@/models/dbModels";
import { useAsyncTask } from "../useAsyncTask";
import { fetchGetItemsList } from "@/services/item";
export function useItems(filter?: ItemFilters) {
    const [items, setItems] = useState<Item[]>([]);
    const { run: load, loading, error } = useAsyncTask(fetchGetItemsList);

    const loadItems = useCallback(async () => {
        try {
            const data = await load(filter);
            setItems(data ?? []);
        } catch {
            setItems([]);
        }
    }, [load, filter]);

    useEffect(() => {
        loadItems();
    }, [loadItems]);

    return { items, setItems, loading, error, reloadItems: loadItems };
}