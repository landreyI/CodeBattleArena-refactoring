import { useEffect, useState } from "react";
import { TaskPlay } from "@/models/dbModels";
import { StandardError } from "@/untils/errorHandler";
import { useAsyncTask } from "../useAsyncTask";
import { fetchGetQuest } from "@/services/quest";

export function useTaskPlay(taskPlayId: number | undefined) {
    const [taskPlay, setTaskPlay] = useState<TaskPlay | null>(null);
    const { run: load, loading, error, setError } = useAsyncTask(fetchGetQuest);

    useEffect(() => {
        if (!taskPlayId) {
            setError(new StandardError("TaskPlay ID not specified"));
            return;
        }

        (async () => {
            const data = await load(taskPlayId);
            setTaskPlay(data);
        })();

    }, [taskPlayId, load]);

    return { taskPlay, setTaskPlay, loading, error };
}