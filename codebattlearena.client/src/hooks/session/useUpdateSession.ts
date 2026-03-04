import { UpdateSessionCommand } from "@/models/dtoCommands";
import { fetchEditSession } from "@/services/session";
import { useAsyncTask } from "../useAsyncTask";
import { useCallback } from "react";

export function useUpdateSession() {
    const { run: update, loading, error } = useAsyncTask(fetchEditSession);
    /**
    * Update session.
    * @param values - The form values for creating a session.
    * @throws StandardError if the session creation fails.
    */
    const updateSession = useCallback(async (
        idSession: string,
        session: UpdateSessionCommand,
    ): Promise<boolean> => {
        const data = await update(idSession, session);
        return data ?? false;
    }, [update]);

    return { updateSession, loading, error };
}
