import { api } from "../api/axios";
import { Player, Session, ProgrammingTask } from "@/models/dbModels";
import { PlayerFilters } from "../models/filters";

// 1. [HttpGet("{id:guid}/sessions")]
export const fetchGetPlayerSessionHistory = async (
    id: string,
    config?: { signal?: AbortSignal }
): Promise<Session[]> => {
    const response = await api.get(`players/${id}/sessions`, config);
    return response.data;
};

// 2. [HttpGet("{id:guid}/programming-tasks")]
export const fetchGetPlayerProgrammingTasks = async (
    id: string,
    config?: { signal?: AbortSignal }
): Promise<ProgrammingTask[]> => {
    const response = await api.get(`players/${id}/programming-tasks`, config);
    return response.data;
};

export const fetchGetPlayer = async (
    id: string,
    config?: { signal?: AbortSignal }
): Promise<{ player: Player; isEdit: boolean }> => {
    const response = await api.get(`/Player/info-player`, {
        params: { id },
        signal: config?.signal,
    });
    return response.data;
};

export const fetchGetPlayersList = async (
    filter?: PlayerFilters,
    config?: { signal?: AbortSignal }
): Promise<Player[]> => {
    const response = await api.get(`/Player/list-players`, {
        params: filter,
        signal: config?.signal,
    });
    return response.data;
};

export const fetchSelectRoles = async (
    idPlayer?: string,
    roles?: string[],
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    const response = await api.put(
        `/Admin/select-roles`,
        { idPlayer, roles },
        { signal: config?.signal } 
    );
    return response.data;
};

export const fetchEditPlayer = async (
    player: Player,
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    const response = await api.put(`/Player/edit-player`, player, {
        signal: config?.signal,
    });
    return response.data;
};

export const fetchChangeActiveItem = async (
    idPlayer?: string,
    idItem?: number,
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    const response = await api.put(
        `/Player/change-active-item`,
        null,
        {
            params: { idPlayer, idItem },
            signal: config?.signal
        }
    );
    return response.data;
};