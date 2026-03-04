import { api } from "../api/axios";
import { AvgCompletionTime, LanguagePopularity, PercentageCompletion, PopularityTask } from "../models/statistics";

export const fetchPopularityLanguagesProgramming = async (
    _: void,
    config?: { signal?: AbortSignal }
): Promise<LanguagePopularity[]> => {
    const response = await api.get(`Statistics/popularity-languages-programming`, {
        signal: config?.signal,
    });
    return response.data;
};

export const fetchAvgTaskCompletionTimeByDifficulty = async (
    _: void,
    config?: { signal?: AbortSignal }
): Promise<AvgCompletionTime[]> => {
    const response = await api.get(`Statistics/avg-task-completion-time-by-difficulty`, {
        signal: config?.signal,
    });
    return response.data;
};

export const fetchPopularityTaskProgramming = async (
    _: void,
    config?: { signal?: AbortSignal }
): Promise<PopularityTask[]> => {
    const response = await api.get(`Statistics/popularity-task-programming`, {
        signal: config?.signal,
    });
    return response.data;
};

export const fetchPercentageCompletionByDifficulty = async (
    _: void,
    config?: { signal?: AbortSignal }
): Promise<PercentageCompletion[]> => {
    const response = await api.get(`Statistics/percentage-completion-by-difficulty`, {
        signal: config?.signal,
    });
    return response.data;
};