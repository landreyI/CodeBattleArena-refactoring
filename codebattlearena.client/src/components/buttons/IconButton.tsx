import { Brackets } from "lucide-react";
import { Button, buttonVariants } from "@/components/ui/button";
import { VariantProps } from "class-variance-authority";

type ButtonVariant = VariantProps<typeof buttonVariants>['variant'];

interface IconButtonProps {
    onClick?: () => void;
    icon?: React.ReactNode;
    variant?: ButtonVariant;
    className?: string;
}

export function IconButton({ icon, onClick, variant = "ghost", className }: IconButtonProps) {
    return (
        <Button
            variant={variant}
            size="icon"
            className={`rounded-10 ml-auto transition-colors ${className}`}
            onClick={(e) => {
                e.stopPropagation();
                e.preventDefault();
                onClick?.();
            }}
        >
            {icon ?? <Brackets className="w-4 h-4" />}
        </Button>
    );
};

export default IconButton;
