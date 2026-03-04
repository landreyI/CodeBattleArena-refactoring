import { useAsyncTask } from "../useAsyncTask";
import { useCallback } from "react";
import { StandardError } from "@/untils/errorHandler";
import { fetchDeleteFriend } from "@/services/friend";

export function useDeleteFriend() {

    const { run, loading, error, setError } = useAsyncTask(fetchDeleteFriend);


    const deleteFriend = useCallback(async (idFriend?: number): Promise<boolean> => {
        if (!idFriend) {
            setError(new StandardError("Friend ID not specified"));
            return false;
        }
        return (await run(idFriend)) ?? false;
    }, [run]);

    return { deleteFriend, error, loading };
}