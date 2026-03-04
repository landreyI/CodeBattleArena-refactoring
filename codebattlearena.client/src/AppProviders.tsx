import { ReactNode } from "react";
import { ThemeProvider } from "./contexts/ThemeContext";
import { SignalRSessionHubProvider } from "./contexts/SignalRSessionHubContext";
import { GlobalSignalRProvider } from "./contexts/GlobalSignalRProvider";
import { ActiveSessionProvider } from "./contexts/ActiveSessionContext";

export const AppProviders = ({ children }: { children: ReactNode }) => {
    return (
        <GlobalSignalRProvider>
            <SignalRSessionHubProvider>
                <ThemeProvider>
                    <ActiveSessionProvider>
                        {children}
                    </ActiveSessionProvider>
                </ThemeProvider>
            </SignalRSessionHubProvider>
        </GlobalSignalRProvider>
    );
};
