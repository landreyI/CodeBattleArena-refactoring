import { createRoot } from "react-dom/client";
import { Circle } from "lucide-react";
import { motion, useAnimation } from "framer-motion";
import { useEffect, ReactNode } from "react";

interface Props {
    icon?: ReactNode;
}

const iconOffsets = [
    { x: -100, y: -50 },
    { x: 80, y: -80 },
    { x: -60, y: 70 },
    { x: 100, y: 50 },
    { x: 0, y: -120 },
    { x: -100, y: -50 },
    { x: 80, y: -80 },
    { x: -60, y: 70 },
    { x: 100, y: 50 },
    { x: 0, y: -120 },
];

export async function showAnimationRotateIcons({ icon }: Props = {}): Promise<void> {
    return new Promise((resolve) => {
        const container = document.createElement("div");
        document.body.appendChild(container);

        const root = createRoot(container);

        root.render(
            <AnimatedRotateIcons
                icon={icon}
                onComplete={() => {
                    root.unmount();
                    container.remove();
                    resolve();
                }}
            />
        );
    });
}

function AnimatedRotateIcons({
    icon,
    onComplete,
}: {
    icon?: ReactNode;
    onComplete: () => void;
}) {
    return (
        <>
            {iconOffsets.map((offset, index) => (
                <SingleAnimatedRotateIcon
                    icon={icon}
                    key={index}
                    offset={offset}
                    delay={index * 0.15}
                    onFinalExit={index === iconOffsets.length - 1 ? onComplete : undefined}
                />
            ))}
        </>
    );
}

function SingleAnimatedRotateIcon({
    icon,
    offset,
    delay,
    onFinalExit,
}: {
    icon?: ReactNode;
    offset: { x: number; y: number };
    delay: number;
    onFinalExit?: () => void;
}) {
    const controls = useAnimation();

    useEffect(() => {
        controls.start({
            scale: 1.5,
            rotate: 360,
            opacity: 1,
            transition: { duration: 1.2, ease: "easeOut", delay },
        });

        const timeout = setTimeout(() => {
            controls
                .start({
                    x: offset.x * 2,
                    y: offset.y * 2,
                    opacity: 0,
                    scale: 0.5,
                    rotate: 720,
                    transition: {
                        duration: 1.5,
                        ease: "easeInOut",
                    },
                })
                .then(() => {
                    setTimeout(() => {
                        onFinalExit?.();
                    }, 300);
                });
        }, 1200 + delay * 1000);

        return () => clearTimeout(timeout);
    }, [controls, offset, delay, onFinalExit]);

    return (
        <motion.div
            initial={{ scale: 0, rotate: 0, opacity: 0 }}
            animate={controls}
            className="fixed top-1/2 left-1/2 pointer-events-none z-50"
            style={{ transform: "translate(-50%, -50%)" }}
        >
            {icon ?? <Circle className="text-yellow-400 w-10 h-10 drop-shadow-xl" />}
        </motion.div>
    );
}
