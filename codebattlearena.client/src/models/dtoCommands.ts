// Соответствует CreateSessionCommand
export interface CreateSessionCommand {
    name: string;
    programmingLangId: string; // Guid
    state: SessionState;
    maxPeople: number;
    password?: string | null;
    timePlay?: number | null;
    taskId?: string | null;
}

// Соответствует UpdateSessionCommand
export interface UpdateSessionCommand {
    id: string;
    name?: string | null;
    programmingLangId?: string | null;
    state?: SessionState | null;
    maxPeople?: number | null;
    password?: string | null;
    timePlay?: number | null;
    taskId?: string | null;
}