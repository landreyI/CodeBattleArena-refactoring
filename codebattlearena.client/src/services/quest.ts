import { api } from "../api/axios";
import { PlayerQuest, Reward, Quest } from "@/models/dbModels";

export const fetchGetQuest = async (
    id: number | undefined,
    config?: { signal?: AbortSignal }
): Promise<TaskPlay> => {
    const response = await api.get(`Quest/info-quest`, {
        params: { id },
        signal: config?.signal,
    });
    return response.data;
};

export const fetchGetQuests = async (
    _: void,
    config?: { signal?: AbortSignal }
): Promise<TaskPlay[]> => {
    const response = await api.get(`Quest/list-quests`, {
        signal: config?.signal,
    });
    return response.data;
};

export const fetchGetListPlayerProgress = async (
    idPlayer: string | undefined,
    config?: { signal?: AbortSignal }
): Promise<PlayerTaskPlay[]> => {
    const response = await api.get(`Quest/list-player-progress`, {
        params: { idPlayer },
        signal: config?.signal,
    });
    return response.data;
};

export const fetchGetPlayerProgress = async (
    idPlayer?: string,
    idTaskPlay?: number,
    config?: { signal?: AbortSignal }
): Promise<PlayerTaskPlay> => {
    const response = await api.get(`Quest/player-progress`, {
        params: {
            idPlayer,
            idTaskPlay,
        },
        signal: config?.signal,
    });
    return response.data;
};

export const fetchGetRewards = async (
    _: void,
    config?: { signal?: AbortSignal }
): Promise<Reward[]> => {
    const response = await api.get(`Quest/list-rewards`, {
        signal: config?.signal,
    });
    return response.data;
};

export const fetchGetTaskPlayRewards = async (
    id: number | undefined,
    config?: { signal?: AbortSignal }
): Promise<Reward[]> => {
    const response = await api.get(`Quest/list-task-play-rewards`, {
        params: { id },
        signal: config?.signal,
    });
    return response.data;
};

export const fetchClaimReward = async (
    idPlayer?: string,
    idTaskPlay?: number,
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    const response = await api.put(
        `Quest/claim-reward`,
        { idPlayer, idTaskPlay },
        { signal: config?.signal }
    );
    return response.data;
};


export const fetchCreateTaskPlay = async (
    taskPlay: TaskPlay,
    idRewards?: number[],
    config?: { signal?: AbortSignal }
): Promise<any> => {
    const response = await api.post(`
        AdminQuest/create-task-play`,
        { taskPlay, idRewards },
        { signal: config?.signal }
    );
    return response.data;
};

export const fetchDeleteTaskPlay = async (
    id: number,
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    const response = await api.delete(`AdminQuest/delete-task-play`, {
        params: { id },
        signal: config?.signal,
    });
    return response.data;
};

export const fetchUpdateTaskPlay = async (
    taskPlay: TaskPlay,
    idRewards?: number[],
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    const response = await api.put(
        `AdminQuest/edit-task-play`,
        { taskPlay, idRewards },
        { signal: config?.signal }
    );
    return response.data;
};

export const fetchCreateReward = async (
    reward: Reward,
    config?: { signal?: AbortSignal }
): Promise<any> => {
    const response = await api.post(`AdminQuest/create-reward`, reward, {
        signal: config?.signal,
    });
    return response.data;
};

export const fetchDeleteReward = async (
    id: number,
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    const response = await api.delete(`AdminQuest/delete-reward`, {
        params: { id },
        signal: config?.signal,
    });
    return response.data;
};

export const fetchUpdateReward = async (
    reward: Reward,
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    const response = await api.put(`AdminQuest/edit-reward`, reward, {
        signal: config?.signal,
    });
    return response.data;
};