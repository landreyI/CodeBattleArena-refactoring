import { useSessionsList } from "@/hooks/session/useSessionsList";
import SessionList from "@/components/lists/SessionsList";
import EmptyState from "@/components/common/EmptyState";
import ErrorMessage from "@/components/common/ErrorMessage";
import LoadingScreen from "@/components/common/LoadingScreen";
import { useSessionEventsHub } from "@/hooks/hubs/session/useSessionEventsHub";
import { useLocation } from "react-router-dom";
import { SessionFilters } from "@/models/filters";
import { useState } from "react";
import { parseEnumParam } from "@/untils/helpers";
import { SessionState } from "@/models/dbModels";
import SessionFilter from "@/components/filters/SessionFilter";

export function SessionsList() {
    const location = useLocation();
    const queryParams = new URLSearchParams(location.search);

    const idLang = queryParams.get('idLang') ? Number(queryParams.get('idLang')) : undefined;
    const maxPeople = Number(queryParams.get('maxPeople') ?? "10");
    const sessionState = parseEnumParam(queryParams.get('sessionState'), SessionState, SessionState.Public);
    const isStart = Boolean(queryParams.get('isStart') ?? "");
    const isFinish = Boolean(queryParams.get('isFinish') ?? "");

    const filterReceived: SessionFilters = {
        idLang,
        maxPeople,
        sessionState,
        isStart,
        isFinish,
    };

    const [filter, setFilter] = useState<SessionFilters>(filterReceived);

    const { sessions, setSessions, loading: sessionsLoad, error: sessionsError, reloadSessions } = useSessionsList(filter);

    useSessionEventsHub(undefined, {
        onListDelete: (sessionId) =>
            setSessions((prevSessions) => prevSessions.filter((session) => session.id !== sessionId)),
        onAdding: (addSession) => setSessions((prevSessions) => [addSession, ...prevSessions]),
        onListUpdate: (updateSession) => 
            setSessions((prevSessions) =>
                prevSessions.map((session) =>
                    session.id === updateSession.id ? updateSession : session
                )
            ),
    });

    const handleChangeFilter = (filter: SessionFilters) => {
        setFilter(filter);
    }

    if (sessionsLoad) return <LoadingScreen />
    if (sessionsError) return <ErrorMessage error={sessionsError} />;

    return (
        <>
            <SessionFilter filter={filter} onChange={handleChangeFilter} handleSearch={reloadSessions}></SessionFilter>

            {(!sessions || sessions.length === 0) && (<EmptyState message="Sessions not found" />)}

            <SessionList
                sessions={sessions}
                className="mt-4"
                cardWrapperClassName="hover:scale-[1.02] transition"
            />
        </>
    )
}

export default SessionsList;