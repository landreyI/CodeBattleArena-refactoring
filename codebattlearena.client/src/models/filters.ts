import { Difficulty, SessionState, TypeItem } from "./dbModels";

export interface PlayerFilters {
    role?: string;
    userName?: string;
}

export interface TaskProgrammingFilters {
    idLang?: number;
    difficulty?: Difficulty;
}

export interface SessionFilters {
    idLang?: number;
    maxPeople?: number;
    sessionState?: SessionState;
    isStart: boolean;
    isFinish: boolean;
}

export interface ItemFilters {
    name?: string;
    type?: TypeItem;
    coin?: number;
    isCoinDescending: boolean;
}