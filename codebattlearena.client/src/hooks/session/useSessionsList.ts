import { useEffect, useState } from "react";
import { fetchGetSessionsList } from "@/services/session";
import { Session } from "@/models/dbModels";
import { useCallback } from "react";
import { useAsyncTask } from "../useAsyncTask";
import { SessionFilters } from "@/models/filters";

export function useSessionsList(filter?: SessionFilters) {
    const [sessions, setSessions] = useState<Session[]>([]);
    const { run: load, loading, error } = useAsyncTask(fetchGetSessionsList);

    const loadSessions = useCallback(async () => {

        try {
            const data = await load(filter);
            setSessions(data ?? []);
        } catch {
            setSessions([]);
        }
    }, [load, filter]);

    useEffect(() => {
        loadSessions();
    }, [loadSessions]);

    return { sessions, setSessions, loading, error, reloadSessions: loadSessions };
}