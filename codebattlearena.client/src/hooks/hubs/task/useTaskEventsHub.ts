import { ProgrammingTask } from "@/models/dbModels";
import { useEffect } from "react";
import { useGlobalSignalR, useGlobalSignalREvent } from "@/contexts/GlobalSignalRProvider";

// --- ТИПЫ ---

export interface MetadataTaskGenerationStatus {
    task: ProgrammingTask;
    error?: string;
}

export interface LanguageGenerationStatus {
    langId: string;
    langName: string;
    success: boolean;
    error?: string;
}

interface TaskEventHandlers {
    onDelete?: (id: string) => void;
    onUpdate?: (task: ProgrammingTask) => void;
    onAdding?: (task: ProgrammingTask) => void;
    onListUpdate?: (task: ProgrammingTask) => void;
    onListDelete?: (id: string) => void;
    onMetadataReady?: (status: MetadataTaskGenerationStatus) => void;
    onLanguageStatus?: (status: LanguageGenerationStatus) => void;
}

/**
 * Хук для отслеживания событий задач и ИИ-генерации.
 * @param identifier GUID задачи или процесса генерации
 * @param type 'job' для генерации ИИ, 'task' для обновлений существующей задачи
 * @param handlers Колбэки для обработки событий
 */
export function useTaskEventsHub(
    identifier: string | undefined,
    type: 'job' | 'task',
    handlers: TaskEventHandlers
) {
    const connection = useGlobalSignalR();

    // --- ЛОГИКА ПОДПИСКИ НА ГРУППУ ---
    useEffect(() => {
        if (!connection || !identifier) return;

        const joinMethod = type === 'job' ? 'SubscribeToJob' : 'JoinTaskGroup';
        const leaveMethod = type === 'task' ? 'LeaveTaskGroup' : null;

        connection.invoke(joinMethod, identifier)
            .catch(err => console.error(`SignalR: Error joining ${type} group:`, err));

        return () => {
            if (leaveMethod && connection.state === "Connected") {
                connection.invoke(leaveMethod, identifier).catch(() => { });
            }
        };
    }, [connection, identifier, type]);

    // --- РЕГИСТРАЦИЯ СОБЫТИЙ ---

    // События для конкретной задачи/генерации (внутри группы)
    useGlobalSignalREvent("TaskDeleting", (id: string) => handlers.onDelete?.(id));
    useGlobalSignalREvent("TaskUpdated", (task: ProgrammingTask) => handlers.onUpdate?.(task));
    useGlobalSignalREvent("MetadataReady", (status: MetadataTaskGenerationStatus) => handlers.onMetadataReady?.(status));
    useGlobalSignalREvent("LanguageStatus", (status: LanguageGenerationStatus) => handlers.onLanguageStatus?.(status));

    // Глобальные события (для всех пользователей)
    useGlobalSignalREvent("TaskAdding", (task: ProgrammingTask) => handlers.onAdding?.(task));
    useGlobalSignalREvent("TasksListUpdated", (task: ProgrammingTask) => handlers.onListUpdate?.(task));
    useGlobalSignalREvent("TasksListDeleting", (id: string) => handlers.onListDelete?.(id));
}