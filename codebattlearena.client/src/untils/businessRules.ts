import { Role, PlayerSession, TaskParamKey, TaskPlay } from "@/models/dbModels";

export const isModerationRole = (roles: string[]): boolean => {
    return roles.some(role => role === Role.Moderator || role === Role.Admin);
};

export const isAdminRole = (roles: string[]): boolean => {
    return roles.some(role => role === Role.Admin);
};

export const isEditRole = (roles: string[]): boolean => {
    return roles.some(role => role === Role.Manager || role === Role.Admin);
};

export const isBannedRole = (roles: string[]): boolean => {
    return roles.some(role => role === Role.Banned);
};

export const isPlayerInSession = (playerId: string, playerSessions: [PlayerSession]): boolean => {
    return playerSessions.some(player => player.idPlayer === playerId);
}
export function getTaskParam(task: TaskPlay | undefined, key: TaskParamKey): string | undefined {
    return task?.taskPlayParams?.find(p => p.paramKey === key)?.paramValue;
}
export function getTaskParamPrimary(task: TaskPlay | undefined): string | undefined {
    return task?.taskPlayParams?.find(p => p.isPrimary)?.paramValue;
}