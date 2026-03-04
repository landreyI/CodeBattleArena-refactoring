export interface LanguagePopularity {
    language?: string
    sessions: number
}
export interface AvgCompletionTime {
    difficulty?: string
    time?: number
}
export interface PopularityTask {
    task?: string
    usage?: number
}
export interface PercentageCompletion {
    difficulty?: string
    percent?: number
}