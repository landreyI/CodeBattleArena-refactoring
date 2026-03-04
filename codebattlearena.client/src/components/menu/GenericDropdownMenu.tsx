// components/common/GenericDropdownMenu.tsx
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuLabel,
    DropdownMenuSeparator,
    DropdownMenuGroup,
    DropdownMenuItem,
    DropdownMenuShortcut,
} from "@/components/ui/dropdown-menu";
import React from "react";
import { useNavigate } from "react-router-dom";

export interface MenuAction {
    label: string;
    shortcut?: string;
    href?: string;
    onClick?: (e: React.MouseEvent<HTMLElement>) => void;
    isSeparator?: boolean;
}

interface GenericDropdownMenuProps {
    triggerContent: React.ReactNode;
    menuLabel: string;
    actions: MenuAction[];
    className?: string;
}

export function GenericDropdownMenu({ triggerContent, menuLabel, actions, className }: GenericDropdownMenuProps) {
    const navigate = useNavigate();

    return (
        <DropdownMenu>
            <DropdownMenuTrigger asChild>
                {triggerContent}
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className={`border ${className}`}>
                <DropdownMenuLabel>{menuLabel}</DropdownMenuLabel>
                <DropdownMenuSeparator />
                <DropdownMenuGroup>
                    {actions.map((action, idx) => (
                        <React.Fragment key={idx}>
                            {action.href ? (
                                <DropdownMenuItem onClick={() => navigate(`${action.href}`)}>
                                    {action.label}
                                    {action.shortcut && <DropdownMenuShortcut>{action.shortcut}</DropdownMenuShortcut>}
                                </DropdownMenuItem>
                            ) : (
                                <DropdownMenuItem
                                        onClick={
                                            action.onClick
                                                ? (e) => setTimeout(() => action.onClick!(e), 0)
                                                : undefined
                                        }
                                >
                                    {action.label}
                                    {action.shortcut && <DropdownMenuShortcut>{action.shortcut}</DropdownMenuShortcut>}
                                </DropdownMenuItem>
                            )}
                            {action.isSeparator && <DropdownMenuSeparator />}
                        </React.Fragment>
                    ))}
                </DropdownMenuGroup>
            </DropdownMenuContent>
        </DropdownMenu>
    );
}
