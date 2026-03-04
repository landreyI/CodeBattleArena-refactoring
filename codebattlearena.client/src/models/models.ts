import { Difficulty, ProgrammingLang } from "./dbModels";

export interface GenerateAITaskProgramming {
    idTaskProgramming?: number;

    langProgrammingId: number;
    langProgramming: ProgrammingLang | null;

    difficulty: Difficulty;
    promt?: string | null;
}