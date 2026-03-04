import { useEffect, useState } from "react";
import { LangProgramming } from "@/models/dbModels";
import { fetchGetListLangsProgramming } from "@/services/langProgramming";
import { useAsyncTask } from "./useAsyncTask";
export function useLangsProgramming() {
    const [langsProgramming, setLangsProgramming] = useState<LangProgramming[] | null>(null);
    const { run: load, loading, error } = useAsyncTask(fetchGetListLangsProgramming);

    useEffect(() => {
        (async () => {
            const data = await load();
            setLangsProgramming(data);
        })();
    }, [load]);

    return { langsProgramming, setLangsProgramming, loading, error };
}