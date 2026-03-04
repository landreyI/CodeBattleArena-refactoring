import { Difficulty, Role, SessionState, LeagueEnum, TypeItem, Item, Player, GameStatus } from "../models/dbModels";
import { StandardError } from "./errorHandler";


export const getArray = (input: any): any[] => {
    if (Array.isArray(input)) return input;
    if (input?.$values) return input.$values;
    return [];
};

export function isQuestResetAvailable(
    completedAt?: Date,
    repeatAfterDays?: number
): boolean {
    if (!completedAt || !repeatAfterDays) return false;

    const completedDate = new Date(completedAt);
    const resetDate = new Date(completedDate);
    resetDate.setDate(resetDate.getDate() + repeatAfterDays);

    return new Date() >= resetDate;
}
export function getProgressDisplay(progress: string | null | undefined, target?: string) {
    return (progress ? `Current: ${progress}` : "No progress") + ` : ${ target }`;
}

export function getLevelData(totalExp: number) {
    let level = 1;
    let expForNextLevel = 100;
    let remainingExp = totalExp;

    while (remainingExp >= expForNextLevel) {
        remainingExp -= expForNextLevel;
        level++;
        expForNextLevel = Math.floor(expForNextLevel * 1.2);
    }

    const progress = Math.floor((remainingExp / expForNextLevel) * 100);

    return {
        level,
        expToNextLevel: expForNextLevel,
        currentExp: remainingExp,
        progressPercent: progress,
    };
}

export function formatDuration(finish: Date, start: Date): string {
    const diffMs = new Date(finish).getTime() - new Date(start).getTime();
    if (diffMs < 0) return "Invalid";

    const totalSeconds = Math.floor(diffMs / 1000);
    const minutes = Math.floor(totalSeconds / 60);
    const seconds = totalSeconds % 60;

    return `${minutes}m ${seconds}s`;
}

export const typeItemClassMap: Record<TypeItem, string> = {
    [TypeItem.Background]: "aspect-[4/3] w-full max-w-[400px] min-w-[150px]",

    [TypeItem.Avatar]: "relative w-[150px] h-[150px] border scale-[1]",
    [TypeItem.Badge]: "w-[60px] h-[60px]",
    [TypeItem.Border]: "w-[150px] h-[150px]",
    [TypeItem.Title]: "w-full h-auto mt-7",
};

export const isActiveItem = (idItem?: number, player?: Player): boolean => {
    return [
        player?.activeBackgroundId,
        player?.activeAvatarId,
        player?.activeBadgeId,
        player?.activeBorderId,
        player?.activeTitleId,
    ].includes(idItem);
};

export function parseEnumParam<T extends Record<string, string>>(
    value: string | null,
    enumType: T,
    defaultValue: T[keyof T]
): T[keyof T] {
    if (value && Object.values(enumType).includes(value as T[keyof T])) {
        return value as T[keyof T];
    }
    return defaultValue;
}

export const getStateColor = (state: string) => {
    switch (state) {
        case SessionState.Public:
            return "bg-green text-black";
        case SessionState.Private:
            return "bg-red text-black";
        default:
            return "bg-gray";
    }
};

export const getRoleColor = (role: string): string => {
    switch (role) {
        case Role.Admin:
            return "bg-yellow";
        case Role.Manager:
            return "bg-blue";
        case Role.Moderator:
            return "bg-purple";
        case Role.Banned:
            return "bg-red";
        case Role.User:
            return "bg-green";
        default:
            return "bg-gray"; // дефолтный стиль для неизвестных ролей
    }
};

export const getDifficultyColor = (difficulty: string) => {
    switch (difficulty) {
        case Difficulty.Easy:
            return "bg-green text-black";
        case Difficulty.Medium:
            return "bg-yellow text-black";
        case Difficulty.Hard:
            return "bg-red text-black";
        default:
            return "bg-gray";
    }
};

export const getIsStartGameColor = (status: GameStatus) => {
    return status === GameStatus.Waiting ? "bg-green" : "bg-red" 
};

export const getClassTypeItem = (type: string) => {
    switch (type) {
        case TypeItem.Background:
            return "";
        case TypeItem.Avatar:
            return "";
        case TypeItem.Badge:
            return "";
        case TypeItem.Border:
            return "";
        case TypeItem.Title:
            return "";
        default:
            return "";
    }
}
