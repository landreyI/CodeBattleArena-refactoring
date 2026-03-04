import { motion, useMotionValue, useSpring } from "framer-motion";
import { Link } from "react-router-dom";
import { Badge } from "../components/ui/badge";
import { Button } from "../components/ui/button";

interface Logo {
    src: string;
    alt: string;
}

const logos: Logo[] = [
    { src: 'https://cdn.jsdelivr.net/gh/devicons/devicon/icons/c/c-original.svg', alt: 'C' },
    { src: 'https://cdn.jsdelivr.net/gh/devicons/devicon/icons/cplusplus/cplusplus-original.svg', alt: 'C++' },
    { src: 'https://cdn.jsdelivr.net/gh/devicons/devicon/icons/csharp/csharp-original.svg', alt: 'C#' },
    { src: 'https://cdn.jsdelivr.net/gh/devicons/devicon/icons/java/java-original.svg', alt: 'Java' },
    { src: 'https://cdn.jsdelivr.net/gh/devicons/devicon/icons/javascript/javascript-original.svg', alt: 'JavaScript' },
    { src: 'https://cdn.jsdelivr.net/gh/devicons/devicon/icons/python/python-original.svg', alt: 'Python' },
    { src: 'https://cdn.jsdelivr.net/gh/devicons/devicon/icons/ruby/ruby-original.svg', alt: 'Ruby' },
    { src: 'https://cdn.jsdelivr.net/gh/devicons/devicon/icons/swift/swift-original.svg', alt: 'Swift' },
];

function getRandomOffset(max: number = 20): number {
    return (Math.random() - 0.5) * 2 * max;
}

interface OrbitingLogoProps {
    index: number;
    total: number;
    logo: Logo;
    side: 'left' | 'right';
}

function OrbitingLogo({ index, total, logo, side }: OrbitingLogoProps) {
    const angle = side === 'left'
        ? (index / (total - 1)) * Math.PI + Math.PI / 2 // Left: π/2 to 3π/2
        : (index / (total - 1)) * Math.PI - Math.PI / 2; // Right: -π/2 to π/2
    const radiusX = 10; // Horizontal radius for the semicircle
    const radiusY = 40; // Vertical radius for the semicircle
    const centerX = side === 'left' ? 15 : 85; // Left side at 20%, right side at 80%
    const centerY = 50; // Centered vertically

    const xPercent = centerX + radiusX * Math.cos(angle);
    const yPercent = centerY + radiusY * Math.sin(angle);

    const offsetX = useMotionValue(0);
    const offsetY = useMotionValue(0);

    const springX = useSpring(offsetX, { stiffness: 120, damping: 10 });
    const springY = useSpring(offsetY, { stiffness: 120, damping: 10 });

    return (
        <motion.div
            className="absolute"
            style={{
                left: `${xPercent}%`,
                top: `${yPercent}%`,
                x: '-50%',
                y: '-50%',
                translateX: springX,
                translateY: springY,
            }}
            animate={{
                rotate: [0, 5, -5, 0],
                scale: [1, 1.05, 0.97, 1.03, 1],
            }}
            transition={{
                rotate: { repeat: Infinity, duration: 4 + (index % 3), ease: 'easeInOut', delay: index * 0.2 },
                scale: { repeat: Infinity, duration: 4 + (index % 3), ease: 'easeInOut', delay: index * 0.2 },
            }}
            onMouseEnter={() => {
                offsetX.set(getRandomOffset());
                offsetY.set(getRandomOffset());
            }}
            onMouseLeave={() => {
                offsetX.set(0);
                offsetY.set(0);
            }}
        >
            <img
                src={logo.src}
                alt={logo.alt}
                style={{
                    width: 'clamp(35px, 5vw, 70px)',
                    height: 'clamp(35px, 5vw, 70px)',
                    objectFit: 'contain',
                    pointerEvents: 'auto',
                }}
            />
        </motion.div>
    );
}

export function HomePage() {
    const halfCount = Math.ceil(logos.length / 2); // Split logos into two groups

    return (
        <div className="relative w-full h-fit p-1 flex items-center justify-center overflow-hidden">
            {/* Орбитальные иконки (только десктоп) */}
            <div className="hidden md:block absolute inset-0 z-0 pointer-events-none">
                {logos.slice(0, halfCount).map((logo, index) => (
                    <OrbitingLogo key={`left-${index}`} index={index} total={halfCount} logo={logo} side="left" />
                ))}
                {logos.slice(halfCount).map((logo, index) => (
                    <OrbitingLogo key={`right-${index}`} index={index} total={logos.length - halfCount} logo={logo} side="right" />
                ))}
            </div>

            <div className="flex flex-col max-w-2xl z-10 items-center text-center my-4 p-2 gap-8">
                <h1 className="text-4xl md:text-5xl font-extrabold uppercase glow-3d text-primary">
                    Welcome<br />Code Battle Arena
                </h1>

                <h2 className="text-xl md:text-2xl font-mono">
                    <span>Multiplayer Coding Arena • </span>
                    <span className="text-primary">Level Up Your Skills</span>
                </h2>

                <p className="text-sm md:text-base font-mono leading-relaxed opacity-90">
                    Battle through real-time challenges. Train your logic, master algorithms,<br />
                    climb ranks, and dominate the coding leagues.
                </p>

                <div className="flex flex-wrap justify-center gap-2">
                    {[
                        'Live Duels',
                        'Code PvP',
                        'Quests & XP',
                        'Leaderboard',
                        'Rewards',
                        'Team vs Team',
                        'Draft Tasks',
                    ].map((label) => (
                        <Badge
                            key={label}
                            className="px-4 py-1 text-xs md:text-sm tracking-wide rounded-xl bg-primary/10 text-primary border border-primary/20 font-mono text-center min-w-[120px] md:min-w-0 transform transition duration-300 hover:scale-105"
                        >
                            {label}
                        </Badge>
                    ))}
                </div>

                <Link
                    to="/session/create-session"
                    className="inline-block relative px-4 py-2 border border-primary border-2 text-primary rounded-xl btn-animation hover:bg-primary hover:text-background"
                    style={{
                        filter: 'drop-shadow(0 0 6px var(--color-primary))',
                        textShadow: '0 0 1px var(--color-primary), 0 0 3px var(--color-primary)',
                    }}
                >
                    Launch Battle
                </Link>
            </div>
        </div>
    );
}

export default HomePage;