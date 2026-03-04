import "./App.css";
import "./items.css"
import { BrowserRouter } from 'react-router-dom';
import Header from './components/layout/Header';
import Footer from './components/layout/Footer';
import Bubbles from './components/layout/Bubbles';
import PanelSidebar from "./components/adminPanel/PanelSidebar";
import { SidebarProvider, SidebarTrigger } from "./components/ui/sidebar";
import { Toaster } from "sonner";
import { AppProviders } from "./AppProviders";
import { AppRoutes } from "./AppRoutes";
import PlayerHubListener from "./components/hubs/PlayerHubListener";
import TooManyRequestsOverlay from "./components/common/TooManyRequestsOverlay";
import { registerTooManyRequestsHandler } from "./api/axios";
import { useEffect, useState } from "react";

const App = () => {
    const [showOverlay, setShowOverlay] = useState(false);

    useEffect(() => {
        registerTooManyRequestsHandler(() => setShowOverlay(true));
    }, []);

    return (
        <>
            <div className="app-container">
                <BrowserRouter>
                    <AppProviders>
                        <PlayerHubListener />
                        <Bubbles />
                        <SidebarProvider className="flex min-h-screen flex-row" defaultOpen={false}>
                            <PanelSidebar />
                            <div className="flex flex-col flex-1">
                                <Header />
                                <SidebarTrigger className="block md:hidden z-10 mt-3 ml-3" iconSize={25} />
                                {showOverlay && (
                                    <TooManyRequestsOverlay onReload={() => window.location.reload()} />
                                )}

                                <main className="app-main" style={{ zIndex: 1 }}>
                                    <AppRoutes />
                                </main>
                                <Footer />
                            </div>
                        </SidebarProvider>
                    </AppProviders>
                </BrowserRouter>
            </div>
            <Toaster />
        </>
    );
};

export default App;
