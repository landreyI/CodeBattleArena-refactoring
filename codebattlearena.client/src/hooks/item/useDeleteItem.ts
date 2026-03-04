import { fetchDeleteItem } from "@/services/item";
import { useAsyncTask } from "../useAsyncTask";
import { useCallback } from "react";
import { StandardError } from "@/untils/errorHandler";

export function useDeleteItem() {

    const { run, loading, error, setError } = useAsyncTask(fetchDeleteItem);


    const deleteItem = useCallback(async (itemId?: number): Promise<boolean> => {
        if (!itemId) {
            setError(new StandardError("Item ID not specified"));
            return false;
        }
        return (await run(itemId)) ?? false;
    }, [run]);

    return { deleteItem, error, loading };
}