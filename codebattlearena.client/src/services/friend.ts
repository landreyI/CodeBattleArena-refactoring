import { api } from "../api/axios";
import { Friendship } from "../models/dbModels";

export const fetchGetListFriends = async (
    _: void,
    config?: { signal?: AbortSignal }
): Promise<Friend[]> => {
    const response = await api.get(`Friend/list-friends`, {
        signal: config?.signal,
    });
    return response.data;
};

export const fetchGetFriendshipFriends = async (
    _: void,
    config?: { signal?: AbortSignal }
): Promise<Friend[]> => {
    const response = await api.get(`Friend/friendship-friends`, {
        signal: config?.signal,
    });
    return response.data;
};

export const fetcAddFriend = async (
    addresseeId?: string,
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    const response = await api.post(`Friend/add-friend`, addresseeId, {
        signal: config?.signal,
    });
    return response.data;
};

export const fetchApproveFriendship = async (
    idFriend?: number,
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    const response = await api.put(`/Friend/approve-friendship`, idFriend, {
        signal: config?.signal,
    });
    return response.data;
};

export const fetchDeleteFriend = async (
    idFriend?: number,
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    const response = await api.delete(`Friend/delete-friend`, {
        params: { idFriend },
        signal: config?.signal,
    });
    return response.data;
};
