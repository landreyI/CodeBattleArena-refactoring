import { Maximize, Minimize } from "lucide-react";
import { Button } from "@/components/ui/button";

interface ToggleSizeButtonProps {
    fullScreen: boolean;
    onClick: () => void;
}

export function ToggleSizeButton ({ fullScreen, onClick }: ToggleSizeButtonProps) {
    return (
        <Button
            onClick={onClick}
            size="icon"
            variant="ghost"
            title={fullScreen ? "Collapse" : "Expand"}
            className="btn-gray size-6"
        >
            {fullScreen ? <Minimize size={16} /> : <Maximize size={16} />}
        </Button>
    );
};
