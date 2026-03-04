import { useEffect, useState } from "react";
import { TaskProgramming } from "@/models/dbModels";
import { StandardError } from "@/untils/errorHandler";
import { fetchGetTask } from "@/services/task";
import { useAsyncTask } from "../useAsyncTask";

export function useTask(taskId: number | undefined) {
    const [task, setTask] = useState<TaskProgramming | null>(null);
    const { run: load, loading, error, setError } = useAsyncTask(fetchGetTask);

    useEffect(() => {
        if (!taskId) {
            setError(new StandardError("Task ID not specified"));
            return;
        }

        (async () => {
            const data = await load(taskId);
            setTask(data);
        })();

    }, [taskId, load]);

    return { task, setTask, loading, error };
}