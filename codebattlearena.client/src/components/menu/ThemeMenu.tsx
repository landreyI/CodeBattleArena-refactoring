import { BubblesColors, BasicColor, useTheme } from "@/contexts/ThemeContext";
import { Sun, Moon, SunMoon, CheckCircle, XCircle } from "lucide-react";
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuLabel,
    DropdownMenuSeparator,
    DropdownMenuGroup,
    DropdownMenuItem,
    DropdownMenuSub,
    DropdownMenuSubTrigger,
    DropdownMenuPortal,
    DropdownMenuSubContent,
    DropdownMenuRadioGroup,
    DropdownMenuRadioItem,
} from "@/components/ui/dropdown-menu";

export const ThemeMenu = () => {
    const { isDarkMode, toggleTheme,
            bubblesEnabled, toggleBubbles,
            bubblesColors, setBubblesColors,
            basicColor, setBasicColor
        } = useTheme();


    return (
        <DropdownMenu>
            <DropdownMenuTrigger asChild className="flex items-center gap-2 nav-link">
                <SunMoon size={20} />
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="border">
                <DropdownMenuLabel>Theme</DropdownMenuLabel>
                <DropdownMenuSeparator />
                <DropdownMenuGroup>
                    <DropdownMenuItem onClick={toggleTheme}>
                        {isDarkMode ? (
                            <Sun size={20} />
                        ) : (
                            <Moon size={20} />
                        )}
                        Chenge theme
                    </DropdownMenuItem>
                    <DropdownMenuSub>
                        <DropdownMenuSubTrigger>
                            Chenge basic color
                        </DropdownMenuSubTrigger>
                        <DropdownMenuPortal>
                            <DropdownMenuSubContent>
                                <DropdownMenuRadioGroup
                                    value={basicColor}
                                    onValueChange={(value) => setBasicColor(value as BasicColor)}
                                >
                                    {Object.values(BasicColor).map((color) => (
                                        <DropdownMenuRadioItem key={color} value={color}>
                                            <div
                                                className={`w-4 h-4 rounded bg-${color.toLowerCase()}`}
                                            />
                                        </DropdownMenuRadioItem>
                                    ))}
                                </DropdownMenuRadioGroup>
                            </DropdownMenuSubContent>
                        </DropdownMenuPortal>
                    </DropdownMenuSub>
                </DropdownMenuGroup>
                <DropdownMenuSeparator />
                <DropdownMenuLabel>Bubbles</DropdownMenuLabel>
                <DropdownMenuSeparator />
                <DropdownMenuGroup>
                    <DropdownMenuItem onClick={toggleBubbles}>
                        {bubblesEnabled ? (
                            <>
                                <XCircle size={20} className="mr-2" />
                                <span>Turn off bubbles</span>
                            </>
                        ) : (
                            <>
                                <CheckCircle size={20} className="mr-2" />
                                <span>Turn on bubbles</span>
                            </>
                        )}
                    </DropdownMenuItem>
                    <DropdownMenuSub>
                        <DropdownMenuSubTrigger>
                            Chenge theme
                        </DropdownMenuSubTrigger>
                        <DropdownMenuPortal>
                            <DropdownMenuSubContent>
                                <DropdownMenuRadioGroup
                                    value={bubblesColors}
                                    onValueChange={(value) => setBubblesColors(value as BubblesColors)}
                                >
                                    {Object.values(BubblesColors).map((color) => (
                                        <DropdownMenuRadioItem key={color} value={color}>
                                            <div
                                                className={`w-4 h-4 rounded bg-${color.toLowerCase()}`}
                                            />
                                        </DropdownMenuRadioItem>
                                    ))}
                                </DropdownMenuRadioGroup>
                            </DropdownMenuSubContent>
                        </DropdownMenuPortal>
                    </DropdownMenuSub>
                </DropdownMenuGroup>
            </DropdownMenuContent>
        </DropdownMenu>
    );
}

export default ThemeMenu;
