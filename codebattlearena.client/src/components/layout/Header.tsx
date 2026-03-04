import { useNavigate } from "react-router-dom";
import { useState } from "react";
import { useAuth } from "@/contexts/AuthContext";
import NavLink from "../common/NavLink";
import { RegistrationModal } from "../modals/RegistrationModal";
import AuthorizationModal from "../modals/AuthorizationModal";
import { Coins, Menu, Star, X } from "lucide-react";
import SessionMenu from "../menu/SessionMenu";
import UserMenu from "../menu/UserMenu";
import SessionActiveMenu from "./SessionActiveMenu";
import ThemeMenu from "../menu/ThemeMenu";
import React from "react";
import { Link } from "react-router-dom";

export function Header() {
    const { user, logout } = useAuth();
    const [showRegistration, setShowRegistration] = useState(false);
    const [showAuthorization, setShowAuthorization] = useState(false);

    const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
    const navigate = useNavigate();

    const isAuthenticated = !!user;

    const handleLogout = (e: React.MouseEvent) => {
        e.preventDefault();
        logout();
        navigate("/home");
    };

    return (
        <>
            <header className="header">
                <nav className="w-full mx-auto flex flex-wrap justify-between items-center">
                    <div className="flex items-center gap-3">
                        <button className="md:hidden" onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}>
                            {isMobileMenuOpen ? <X /> : <Menu />}
                        </button>
                        <div className="hidden md:flex items-center gap-8 ml-6">
                            <Link to="/home" title="home" className="text-2xl font-bold rounded-sm">
                                <img src="/public/images/logo.png" alt="avatar" className="h-10 bg-contain bg-no-repeat bg-center" />
                            </Link>
                            <NavLink href="/info" label="Info" />
                            <SessionMenu />
                            <NavLink href="/task/list-task" label="Tasks" />
                            <NavLink href="/league/list-leagues" label="Leagues" />
                            <NavLink href="/item/list-items" label="Items" />
                            <NavLink href="/quest/list-quests" label="Quests" />
                            <NavLink href="/statistics/info-statistics" label="Statistics" />
                        </div>
                    </div>
                    <div className="flex items-center justify-end ml-auto">
                        {isAuthenticated && (
                            <div className="flex items-center gap-3 justify-end ml-auto">
                                <div className="flex flex-wrap ml-3"><Coins className="w-5 h-5 text-yellow shrink-0" /> {user?.coin ?? 0}</div>
                                <div className="flex flex-wrap mr-3"><Star className="w-5 h-5 text-purple shrink-0" /> {user?.experience ?? 0}</div>
                            </div>
                        )}

                        {!isAuthenticated ? (
                            <div className="flex flex-wrap items-center gap-3 justify-end mr-3">
                                <button onClick={() => setShowAuthorization(true)} className="text-primary nav-link">Sign In</button>
                                <button onClick={() => setShowRegistration(true)} className="text-primary nav-link">Sign Up</button>
                            </div>
                        ) : (
                            <UserMenu user={user} handleLogout={handleLogout} classNameBtn="mr-2 hover:text-primary transition-colors duration-300"></UserMenu>
                        )}

                        <ThemeMenu />
                    </div>
                </nav>

                {/* Mobile menu */}
                {isMobileMenuOpen && (
                    <>
                        <div className="md:hidden flex flex-wrap justify-between gap-3 border-t py-4 px-6">
                            <NavLink href="/home" label="Home" />
                            <NavLink href="/info" label="Info" />
                            <div className="col-span-1 flex justify-start">
                                <SessionMenu />
                            </div>
                            <NavLink href="/task/list-task" label="Tasks" />
                            <NavLink href="/league/list-leagues" label="Leagues" />
                            <NavLink href="/item/list-items" label="Items" />
                            <NavLink href="/quest/list-quests" label="Quests" />
                            <NavLink href="/statistics/info-statistics" label="Statistics" />
                            {!isAuthenticated && (
                                <>
                                    <button onClick={() => { setShowAuthorization(true); setIsMobileMenuOpen(false); }} className="text-primary nav-link">Sign In</button>
                                    <button onClick={() => { setShowRegistration(true); setIsMobileMenuOpen(false); }} className="text-primary nav-link">Sign Up</button>
                                </>
                            )}
                        </div>
                    </>
                )}
            </header>
            <SessionActiveMenu></SessionActiveMenu>

            <RegistrationModal open={showRegistration} onClose={() => setShowRegistration(false)} />
            <AuthorizationModal open={showAuthorization} onClose={() => setShowAuthorization(false)} />
        </>
    );
};

export default React.memo(Header);