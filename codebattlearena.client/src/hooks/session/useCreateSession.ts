import { CreateSessionCommand } from "@/models/dtoCommands";
import { fetchCreateSession } from "@/services/session";
import { useAsyncTask } from "../useAsyncTask";
import { useCallback } from "react";

export function useCreateSession() {
    const { run: create, loading, error } = useAsyncTask(fetchCreateSession);
    /**
    * Creates a new session and returns its ID.
    * @param values - The form values for creating a session.
    * @returns The ID of the created session.
    * @throws StandardError if the session creation fails.
    */
    const createSession = useCallback(async (session: CreateSessionCommand) => {
        const data = await create(session);
        return data;
    }, [create]);

    return { createSession, loading, error };
}
