import { api } from "../api/axios";
import { Session, PlayerSession } from "@/models/dbModels";
import { SessionFilters } from "../models/filters";

// Вспомогательный тип для прогресса
export interface CompletionCount {
    completed: number;
    total: number;
}

// 1. [HttpGet("active")]
export const fetchGetActiveSession = async (
    config?: { signal?: AbortSignal }
): Promise<Session | null> => {
    const response = await api.get(`sessions/active`, config);
    return response.data;
};

// 2. [HttpGet("{id:guid}")]
export const fetchGetSession = async (
    id: string,
    config?: { signal?: AbortSignal }
): Promise<{ session: Session; isEdit: boolean }> => {
    const response = await api.get(`sessions/${id}`, config);
    return response.data;
};

// 3. [HttpGet]
export const fetchGetSessionsList = async (
    filter?: SessionFilters,
    config?: { signal?: AbortSignal }
): Promise<Session[]> => {
    const response = await api.get('sessions', {
        params: filter,
        ...config,
    });
    return response.data;
};

// 4. [HttpPost]
export const fetchCreateSession = async (
    command: any, // CreateSessionCommand
    config?: { signal?: AbortSignal }
): Promise<string> => {
    const response = await api.post('sessions', command, config);
    return response.data;
};

// 5. [HttpPut("{id:guid}")]
export const fetchEditSession = async (
    id: string,
    command: any, // UpdateSessionCommand
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    // Важно: на бэкенде ты проверяешь id !== command.Id
    const response = await api.put(`sessions/${id}`, command, config);
    return response.data;
};

// 6. [HttpDelete("{id:guid}")]
export const fetchDeleteSession = async (
    id: string,
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    // В отличие от старой версии, теперь ID передается в пути (URL)
    const response = await api.delete(`sessions/${id}`, config);
    return response.data;
};

// 7. [HttpGet("{id:guid}/players")]
export const fetchGetSessionPlayers = async (
    id: string,
    config?: { signal?: AbortSignal }
): Promise<PlayerSession[]> => {
    const response = await api.get(`sessions/${id}/players`, config);
    return response.data;
};

// 8. [HttpGet("{sessionId:guid}/players/{playerId:guid}")]
export const fetchGetPlayerSessionInfo = async (
    sessionId: string,
    playerId: string,
    config?: { signal?: AbortSignal }
): Promise<PlayerSession> => {
    const response = await api.get(`sessions/${sessionId}/players/${playerId}`, config);
    return response.data;
};

// 9. [HttpGet("{id:guid}/players/me")]
export const fetchGetMySessionInfo = async (
    id: string,
    config?: { signal?: AbortSignal }
): Promise<PlayerSession> => {
    const response = await api.get(`sessions/${id}/players/me`, config);
    return response.data;
};

// 10. [HttpPut("{id:guid}/join")]
export const fetchJoinSession = async (
    id: string,
    request: { password?: string },
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    const response = await api.put(`sessions/${id}/join`, request, config);
    return response.data;
};

// 11. [HttpPut("unjoin")]
export const fetchUnjoinSession = async (
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    const response = await api.put(`sessions/unjoin`, {}, config);
    return response.data;
};

// 12. [HttpPost("{id:guid}/invite")]
export const fetchInviteSession = async (
    id: string,
    idPlayersInvite: string[],
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    const response = await api.post(`sessions/${id}/invite`, idPlayersInvite, config);
    return response.data;
};

// Если в контроллере будет [HttpDelete("{sessionId:guid}/kick-out/{playerId:guid}")]
export const fetchKickOutSession = async (
    sessionId: string,
    playerId: string,
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    const response = await api.delete(`sessions/${sessionId}/kick-out/${playerId}`, config);
    return response.data;
};

// 14. [HttpPut("{sessionId:guid}/select-task/{taskId:guid}")]
export const fetchSelectTask = async (
    sessionId: string,
    taskId: string,
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    const response = await api.put(`sessions/${sessionId}/select-task/${taskId}`, {}, config);
    return response.data;
};

// 15. [HttpPut("{id:guid}/start-game")]
export const fetchStartGame = async (
    id: string,
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    const response = await api.put(`sessions/${id}/start-game`, {}, config);
    return response.data;
};

// 16. [HttpGet("{id:guid}/completion-count")]
export const fetchGetFinishedPlayersCount = async (
    id: string,
    config?: { signal?: AbortSignal }
): Promise<CompletionCount> => {
    const response = await api.get(`sessions/${id}/completion-count`, config);
    return response.data;
};

// 17. [HttpPut("{id:guid}/finish-game")]
export const fetchFinishGame = async (
    id: string,
    config?: { signal?: AbortSignal }
): Promise<boolean> => {
    const response = await api.put(`sessions/${id}/finish-game`, {}, config);
    return response.data;
};

// 18. [HttpGet("{id:guid}/best-result")]
export const fetchGetBestResult = async (
    id: string,
    config?: { signal?: AbortSignal }
): Promise<PlayerSession> => {
    const response = await api.get(`sessions/${id}/best-result`, config);
    return response.data;
};