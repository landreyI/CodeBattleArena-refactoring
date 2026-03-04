export interface ExecutionResult {
    status?: string;
    stdout?: string[];
    time?: string;
    memory?: number;
    compileOutput?: string;
}