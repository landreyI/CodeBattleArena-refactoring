import { Item } from "@/models/dbModels";
import { useAsyncTask } from "../useAsyncTask";
import { useCallback } from "react";
import { fetchCreateItem } from "@/services/item";

export function useCreateItem() {
    const { run: create, loading, error } = useAsyncTask(fetchCreateItem);
    /**
    * Creates a new item and returns its ID.
    * @param values - The form values for creating a item.
    * @returns The ID of the created item.
    * @throws StandardError if the item creation fails.
    */
    const createItem = useCallback(async (item: Item) => {
        const data = await create(item);
        return data;
    }, [create]);

    return { createItem, loading, error };
}
