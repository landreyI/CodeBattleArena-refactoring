import { useEffect, useRef } from "react";

export function useDebouncedEffect(
    effect: () => Promise<void> | void,
    deps: any[],
    delay: number
) {
    const timeoutRef = useRef<NodeJS.Timeout | null>(null);

    useEffect(() => {
        if (timeoutRef.current) {
            clearTimeout(timeoutRef.current);
        }

        timeoutRef.current = setTimeout(() => {
            // Оборачиваем вызов в async IIFE
            (async () => {
                await effect();
            })();
        }, delay);

        return () => {
            if (timeoutRef.current) {
                clearTimeout(timeoutRef.current);
            }
        };
    }, deps);
}
