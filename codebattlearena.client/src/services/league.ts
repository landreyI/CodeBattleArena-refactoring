import { api } from "../api/axios";
import { League, LeaguePlayers } from "@/models/dbModels";

export const fetchGetLeague = async (
    id: number,
    config?: { signal?: AbortSignal }
): Promise<League> => {
    const response = await api.get(`League/league`, {
        params: { id },
        signal: config?.signal,
    });
    return response.data;
};

export const fetchGetListLeagues = async (
    _: void,
    config?: { signal?: AbortSignal }
): Promise<League[]> => {
    const response = await api.get(`League/list-leagues`, {
        signal: config?.signal,
    });
    return response.data;
};

export const fetchGetLeagueByPlayer = async (
    idPlayer?: string,
    config?: { signal?: AbortSignal }
): Promise<League> => {
    const response = await api.get(`League/league-by-player`, {
        params: { idPlayer },
        signal: config?.signal,
    });
    return response.data;
};

export const fetchGetPlayersLeagues = async (
    _: void,
    config?: { signal?: AbortSignal }
): Promise<LeaguePlayers[]> => {
    const response = await api.get(`League/players-in-leagues`, {
        signal: config?.signal,
    });
    return response.data;
};

export const fetchCreateLeague = async (
    league: League,
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    const response = await api.post(`AdminLeague/create-league`, league, {
        signal: config?.signal,
    });
    return response.data;
};

export const fetchEditLeague = async (
    league: League,
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    const response = await api.put(`AdminLeague/edit-league`, league, {
        signal: config?.signal,
    });
    return response.data;
};

export const fetchDeleteLeague = async (
    id: number,
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    const response = await api.delete(`AdminLeague/delete-league`, {
        params: { id },
        signal: config?.signal,
    });
    return response.data;
};