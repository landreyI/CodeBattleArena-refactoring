import React, { createContext, useContext, useEffect, useState, ReactNode, useCallback } from "react";
import { api } from "../api/axios";

export type UserAuth = {
    id: string;
    roles: string[];
    userName: string;
    photoUrl: string;
    coin?: number | null;
    experience?: number | null;
};

type AuthContextType = {
    user: UserAuth | null;
    isLoading: boolean;
    setUser: React.Dispatch<React.SetStateAction<UserAuth | null>>;
    logout: () => void;
    reload: () => Promise<void>;
};

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
    const [user, setUser] = useState<UserAuth | null>(null);
    const [isLoading, setIsLoading] = useState(true);

    const load = useCallback(async () => {
        const token = localStorage.getItem("accessToken");

        if (!token) {
            setUser(null);
            setIsLoading(false);
            return;
        }

        try {
            const response = await api.get<UserAuth>("/auth/user");
            setUser(response.data);
        } catch (error) {
            console.error("Failed to load user:", error);
            logout();
        } finally {
            setIsLoading(false);
        }
    }, []);

    useEffect(() => {
        load();
    }, [load]);

    const logout = () => {
        localStorage.removeItem("accessToken");
        localStorage.removeItem("refreshToken");

        setUser(null);

        api.post("/auth/logout").catch(() => { });
    };

    return (
        <AuthContext.Provider value={{ user, setUser, logout, reload: load, isLoading }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) throw new Error("useAuth must be used within AuthProvider");
    return context;
};