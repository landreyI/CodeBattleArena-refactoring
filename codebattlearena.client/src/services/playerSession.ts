import { api } from "../api/axios";
import { ExecutionResult } from "@/models/executionResult";

// 1. [HttpPost("check-code")]
export const fetchCheckCode = async (
    code: string,
    config?: { signal?: AbortSignal }
): Promise<ExecutionResult> => {
    const response = await api.post(`games/check-code`, { code }, config);
    return response.data;
};

// 2. [HttpPut("finish-task")]
export const fetchFinishTask = async (
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    const response = await api.put(`games/finish-task`, {}, config);
    return response.data;
};