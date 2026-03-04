import { useEffect, useState, useCallback } from "react";
import { Item } from "@/models/dbModels";
import { StandardError } from "@/untils/errorHandler";
import { useAsyncTask } from "../useAsyncTask";
import { fetchGetItem } from "@/services/item";
export function useItem(itemId: number | undefined) {
    const [item, setItem] = useState<Item | null>(null);
    const { run: load, loading, error, setError } = useAsyncTask(fetchGetItem);

    const loadItem = useCallback(async () => {
        if (!itemId) {
            setError(new StandardError("Item ID not specified"));
            return;
        }

        const data = await load(itemId);
        if (data) {
            setItem(data);
        }
    }, [setError, load, itemId]);

    useEffect(() => {
        loadItem();

    }, [loadItem]);

    return { item, setItem, loading, error, reloadItem: loadItem };
}