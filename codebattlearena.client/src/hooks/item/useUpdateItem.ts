import { Item } from "@/models/dbModels";
import { useAsyncTask } from "../useAsyncTask";
import { useCallback } from "react";
import { fetchUpdateItem } from "@/services/item";

export function useUpdateItem() {
    const { run: update, loading, error } = useAsyncTask(fetchUpdateItem);
    /**
    * Update item.
    * @param values - The form values for creating a item.
    * @throws StandardError if the item creation fails.
    */
    const updateItem = useCallback(async (
        item: Item,
    ): Promise<boolean> => {
        const data = await update(item);
        return data ?? false;
    }, [update]);

    return { updateItem, loading, error };
}
