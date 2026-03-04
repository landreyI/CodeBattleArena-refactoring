import { ReactNode } from "react";
import { createContext, useContext } from "react";
import { Session } from "../models/dbModels";

export const SessionContext = createContext<Session | null>(null);
export const useSession = () => useContext(SessionContext);

export const PlayerProvider = ({ session, children }: { session: Session, children: ReactNode }) => (
    <SessionContext.Provider value={session}>
        {children}
    </SessionContext.Provider>
);