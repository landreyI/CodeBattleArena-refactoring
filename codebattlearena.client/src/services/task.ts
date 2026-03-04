import { api } from "../api/axios";
import { TestCase, ProgrammingTask } from "@/models/dbModels";
import { TaskProgrammingFilters } from "../models/filters";
import { GenerateAITaskProgramming } from "../models/models";

export const fetchGetTask = async (
    id: number,
    config?: { signal?: AbortSignal }
): Promise<TaskProgramming> => {
    const response = await api.get(`Task/info-task-programming`, {
        params: { id },
        signal: config?.signal,
    });
    return response.data;
};

export const fetchGetTasks = async (
    filter: TaskProgrammingFilters | undefined,
    config?: { signal?: AbortSignal }
): Promise<TaskProgramming[]> => {
    const response = await api.get(`Task/list-tasks-programming`, {
        params: filter,
        signal: config?.signal,
    });
    return response.data;
};

export const fetchGetInputDatas = async (
    _: void,
    config?: { signal?: AbortSignal }
): Promise<InputData[]> => {
    const response = await api.get(`Task/list-input-datas`, {
        signal: config?.signal,
    });
    return response.data;
};

export const fetchCreateTask = async (
    task: TaskProgramming,
    config?: { signal?: AbortSignal }
): Promise<any> => {
    const response = await api.post(`AdminTask/create-task-programming`, task, {
        signal: config?.signal,
    });
    return response.data;
};

export const fetchDeleteTask = async (
    id: number,
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    const response = await api.delete(`AdminTask/delete-task-programming`, {
        params: { id },
        signal: config?.signal,
    });
    return response.data;
};

export const fetchUpdateTask = async (
    task: TaskProgramming,
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    const response = await api.put(`AdminTask/edit-task-programming`, task, {
        signal: config?.signal,
    });
    return response.data;
};

export const fetchGetPlayerTasks = async (
    idPlayer?: string,
    config?: { signal?: AbortSignal }
): Promise<TaskProgramming[]> => {
    const response = await api.get(`Task/list-player-tasks`, {
        params: {
            idPlayer,
        },
        signal: config?.signal,
    });
    return response.data;
};

export const fetchGenerateAITask = async (
    task: GenerateAITaskProgramming,
    config?: { signal?: AbortSignal }
): Promise<any> => {
    const response = await api.post(`Task/generate-ai-task-programming`, task, {
        signal: config?.signal,
    });
    return response.data;
};