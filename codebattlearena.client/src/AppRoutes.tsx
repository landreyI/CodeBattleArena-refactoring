import { Suspense } from "react";
import { Routes, Route } from "react-router-dom";
import { routes } from "./routes";
import LoadingScreen from "./components/common/LoadingScreen";

export function AppRoutes() {
    return (
        <Suspense fallback={<LoadingScreen />}>
            <Routes>
                {routes.map((route, index) => (
                    <Route
                        key={index}
                        path={route.path}
                        index={route.index}
                        element={<route.element />}
                    />
                ))}
            </Routes>
        </Suspense>
    );
}
