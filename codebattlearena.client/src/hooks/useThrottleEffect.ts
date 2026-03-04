import { useEffect, useRef } from "react";

export function useThrottleEffect(callback: () => void, deps: any[], limit: number) {
    const lastRan = useRef(Date.now());

    useEffect(() => {
        const handler = setTimeout(() => {
            if (Date.now() - lastRan.current >= limit) {
                callback();
                lastRan.current = Date.now();
            }
        }, limit - (Date.now() - lastRan.current));

        return () => clearTimeout(handler);
    }, deps);
}
