import { createContext, useContext, useEffect, useState, ReactNode } from "react";
import { Session } from "@/models/dbModels";
import { fetchGetActiveSession } from "@/services/session";
import { useMemo } from "react";
import { fetchUnjoinSession } from "@/services/session";

type SessionContextType = {
    activeSession: Session | null;
    setActiveSession: (session: Session) => void;
    leaveSession: () => void;
    refreshSession: () => Promise<void>;
};

const ActiveSessionContext = createContext<SessionContextType | undefined>(undefined);

export function ActiveSessionProvider({ children }: { children: ReactNode }) {
    const [activeSession, setActiveSession] = useState<Session | null>(null);

    const loadActiveSession = async () => {
        try {
            let data = await fetchGetActiveSession();
            setActiveSession(data);
        }
        catch (err){
            console.log(err);
        }
    }

    useEffect(() => {
        loadActiveSession();
    }, []);

    const leaveSession = async () => {
        await fetchUnjoinSession();
        setActiveSession(null);
    }
    const refreshSession = async () => {
        loadActiveSession();
    }

    const value = useMemo(
        () => ({ activeSession, leaveSession, setActiveSession, refreshSession }),
        [activeSession]
    );
    return <ActiveSessionContext.Provider value={value}>{children}</ActiveSessionContext.Provider>;
}

export const useActiveSession = () => {
    const context = useContext(ActiveSessionContext);
    if (!context) throw new Error("useActiveSession  Auth must be used within AuthProvider");
    return context;
};