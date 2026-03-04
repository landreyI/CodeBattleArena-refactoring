import {
    createContext,
    useContext,
    useEffect,
    useState,
    ReactNode,
} from "react";

export enum BubblesColors {
    Green = "green",
    Red = "red",
    Yellow = "yellow",
    Blue = "blue",
    Purple = "purple",
}
export enum BasicColor {
    Green = "green",
    Red = "red",
    Yellow = "yellow",
    Blue = "blue",
    Purple = "purple",
}
interface ThemeContextType {
    isDarkMode: boolean;
    toggleTheme: () => void;

    bubblesEnabled: boolean;
    toggleBubbles: () => void;

    bubblesColors: BubblesColors;
    setBubblesColors: (color: BubblesColors) => void;

    basicColor: BasicColor;
    setBasicColor: (color: BasicColor) => void;
}

export const ThemeContext = createContext<ThemeContextType | undefined>(undefined);

export function ThemeProvider({ children }: { children: ReactNode }) {
    const [isDarkMode, setIsDarkMode] = useState(false);
    const [bubblesEnabled, setBubblesEnabled] = useState(true);
    const [bubblesColors, setBubblesColorsState] = useState<BubblesColors>(BubblesColors.Green);
    const [basicColor, setBasicColorState] = useState<BasicColor>(BasicColor.Green);

    // Load theme from localStorage
    useEffect(() => {
        const savedTheme = localStorage.getItem("theme");
        const prefersDark = window.matchMedia("(prefers-color-scheme: dark)").matches;

        if (savedTheme === "dark" || (!savedTheme && prefersDark)) {
            setIsDarkMode(true);
            document.documentElement.classList.add("dark");
        }
    }, []);

    // Save theme to localStorage
    useEffect(() => {
        if (isDarkMode) {
            document.documentElement.classList.add("dark");
            localStorage.setItem("theme", "dark");
        } else {
            document.documentElement.classList.remove("dark");
            localStorage.setItem("theme", "light");
        }
    }, [isDarkMode]);

    const toggleTheme = () => setIsDarkMode(prev => !prev);

    // Load bubbles enabled/disabled
    useEffect(() => {
        const savedBubbles = localStorage.getItem("bubbles");
        if (savedBubbles === "false") {
            setBubblesEnabled(false);
        }
    }, []);

    useEffect(() => {
        localStorage.setItem("bubbles", bubblesEnabled ? "true" : "false");
    }, [bubblesEnabled]);

    const toggleBubbles = () => setBubblesEnabled(prev => !prev);

    // Load bubbles color
    useEffect(() => {
        const savedColor = localStorage.getItem("bubblesColor") as BubblesColors | null;
        if (savedColor && Object.values(BubblesColors).includes(savedColor as BubblesColors)) {
            setBubblesColorsState(savedColor as BubblesColors);
            document.documentElement.style.setProperty("--color-bubbles", `var(--bubbles-${savedColor})`);
        }
    }, []);

    // Save bubbles color
    const setBubblesColors = (color: BubblesColors) => {
        setBubblesColorsState(color);
        localStorage.setItem("bubblesColor", color);
        document.documentElement.style.setProperty("--color-bubbles", `var(--bubbles-${color})`);
    };

    // Load basic color
    useEffect(() => {
        const savedColor = localStorage.getItem("basicColor") as BasicColor | null;
        if (savedColor && Object.values(BasicColor).includes(savedColor as BasicColor)) {
            setBasicColorState(savedColor as BasicColor);
            document.documentElement.style.setProperty("--color-primary", `var(--primary-${savedColor})`);
        }
    }, []);

    // Save basic color
    const setBasicColor = (color: BasicColor) => {
        setBasicColorState(color);
        localStorage.setItem("basicColor", color);
        document.documentElement.style.setProperty("--color-primary", `var(--primary-${color})`);
    };

    return (
        <ThemeContext.Provider
            value={{
                isDarkMode,
                toggleTheme,
                bubblesEnabled,
                toggleBubbles,
                bubblesColors,
                setBubblesColors,
                basicColor,
                setBasicColor,
            }}
        >
            {children}
        </ThemeContext.Provider>
    );
}

export function useTheme() {
    const context = useContext(ThemeContext);
    if (!context) {
        throw new Error("useTheme must be used within a ThemeProvider");
    }
    return context;
}