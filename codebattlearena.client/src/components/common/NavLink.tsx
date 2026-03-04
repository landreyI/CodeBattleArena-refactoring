import React from 'react';
import { Link } from 'react-router-dom';

interface Props {
    href: string;
    label: string;
    className?: string;
}

export function NavLink({ href, label, className }: Props) {
    return (
        <Link to={href} className={`nav-link text-primary ${className}`}>
            {label}
        </Link>
    );
}

export default NavLink;