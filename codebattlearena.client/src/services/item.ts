import { api } from "../api/axios";
import { Item, PlayerItem, TypeItem } from "../models/dbModels";
import { ItemFilters } from "../models/filters";

export const fetchGetItem = async (
    id: number,
    config?: { signal?: AbortSignal }
): Promise<Item> => {
    const response = await api.get(`Item/info-item`, {
        params: { id },
        signal: config?.signal,
    });
    return response.data;
};

export const fetchGetItemsList = async (
    filter?: ItemFilters,
    config?: { signal?: AbortSignal }
): Promise<Item[]> => {
    const response = await api.get(`Item/list-items`, {
        params: filter,
        signal: config?.signal,
    });
    return response.data;
};

export const fetchGetPlayerItems = async (
    idPlayer?: string,
    typeItem?: TypeItem,
    config?: { signal?: AbortSignal }
): Promise<Item[]> => {
    const response = await api.get(`Item/list-player-items`, {
        params: {
            idPlayer,
            typeItem,
        },
        signal: config?.signal,
    });
    return response.data;
};

export const fetchCreateItem = async (
    item: Item,
    config?: { signal?: AbortSignal }
): Promise<{ idItem: number }> => {
    const response = await api.post(`AdminItem/create-item`, item, {
        signal: config?.signal,
    });
    return response.data;
};

export const fetchBuyItem = async (
    playerItem: PlayerItem,
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    const response = await api.post(`Item/buy-item`, playerItem, {
        signal: config?.signal,
    });
    return response.data;
};

export const fetchUpdateItem = async (
    item: Item,
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    const response = await api.put(`AdminItem/edit-item`, item, {
        signal: config?.signal,
    });
    return response.data;
};

export const fetchDeleteItem = async (
    id: number,
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    const response = await api.delete(`AdminItem/delete-item`, {
        params: { id },
        signal: config?.signal,
    });
    return response.data;
};