import { useState, useCallback, useRef, useEffect } from "react";
import { StandardError, processError } from "@/untils/errorHandler";

const MAX_CALLS = 5;
const WINDOW_MS = 3000;

export function useAsyncTask<T extends any[], R>(
    asyncFunc: (...args: [...T, { signal?: AbortSignal }?]) => Promise<R>
) {
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<StandardError | null>(null);
    const controllerRef = useRef<AbortController | null>(null);
    const versionRef = useRef(0);
    const callTimestampsRef = useRef<number[]>([]);

    const run = useCallback(async (...args: T): Promise<R | null> => {
        const now = Date.now();

        // Очистить старые вызовы
        callTimestampsRef.current = callTimestampsRef.current.filter(
            (ts) => now - ts < WINDOW_MS
        );

        if (callTimestampsRef.current.length >= MAX_CALLS) {
            const standardError = new StandardError("Too much action. Wait a bit.");
            setError(standardError);
            return null;
        }

        callTimestampsRef.current.push(now);

        controllerRef.current?.abort();
        const controller = new AbortController();
        controllerRef.current = controller;

        const currentVersion = ++versionRef.current;
        setLoading(true);
        setError(null);

        try {
            const result = await asyncFunc(...args, { signal: controller.signal });
            return result;
        } catch (err: unknown) {
            if ((err as any).name === "AbortError" || (err as any).code === "ERR_CANCELED") {
                console.log("Fetch aborted:", asyncFunc.name);
                return null;
            }

            if (err?.response?.status === 429) { console.warn("Rate limit triggered"); abort(); return null; }

            const standardError = processError(err);
            if (currentVersion === versionRef.current) {
                setError(standardError);
            }
            throw err;
        } finally {
            if (currentVersion === versionRef.current) {
                setLoading(false);
                controllerRef.current = null;
            }
        }
    }, [asyncFunc]);

    const abort = useCallback(() => {
        if (controllerRef.current && !controllerRef.current.signal.aborted) {
            controllerRef.current.abort();
        }
    }, []);

    // Автоматическая отмена при размонтировании компонента
    useEffect(() => {
        return () => {
            abort();
        };
    }, [abort]);

    return { run, loading, error, setError, abort };
}