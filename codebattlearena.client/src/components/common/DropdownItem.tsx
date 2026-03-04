import React from 'react';
import { Link } from "react-router-dom";
export interface DropdownItemData {
    action?: Function;
    href?: string;
    link?: string;
    label: string;
}

interface Props {
    dropdownItem: DropdownItemData;
}

export function DropdownItem({ dropdownItem }: Props) {
    const className = "px-4 py-2 rounded-md transition-colors";

    if (dropdownItem.href) {
        return (
            <a href={dropdownItem.href} className={className}>
                {dropdownItem.label}
            </a>
        );
    }
    else if (dropdownItem.action) {
        return (
            <button onClick={(e) => dropdownItem.action?.(e)} className={className}>
                {dropdownItem.label}
            </button>
        );
    }
    else if (dropdownItem.link) {
        return (
            <Link to={dropdownItem.link} className={className}>
                {dropdownItem.label}
            </Link>
        );
    }

    return null;
}

export default DropdownItem;