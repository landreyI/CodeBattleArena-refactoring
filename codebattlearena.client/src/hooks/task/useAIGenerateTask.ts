import { GenerateAITaskProgramming } from "@/models/models";
import { fetchGenerateAITask } from "@/services/task";
import { useAsyncTask } from "../useAsyncTask";
import { useCallback } from "react";

export function useAIGenerateTask() {
    const { run: generate, loading, error } = useAsyncTask(fetchGenerateAITask);

    const generateTask = useCallback(async (
        task: GenerateAITaskProgramming
    ) => {
        const idTask = await generate(task);
        return idTask;
    }, [generate])

    return { generateTask, loading, error };
}
