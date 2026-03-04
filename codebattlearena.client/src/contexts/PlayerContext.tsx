import { ReactNode } from "react";
import { createContext, useContext } from "react";
import { Player } from "../models/dbModels";

export const PlayerContext = createContext<Player | null>(null);
export const usePlayer = () => useContext(PlayerContext);

export const PlayerProvider = ({ player, children }: { player: Player | null, children: ReactNode }) => (
    <PlayerContext.Provider value={player}>
        {children}
    </PlayerContext.Provider>
);