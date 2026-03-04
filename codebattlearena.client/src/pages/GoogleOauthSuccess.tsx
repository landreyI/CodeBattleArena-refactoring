import { useEffect } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";

const GoogleOauthSuccess = () => {
    const navigate = useNavigate();
    const { reload } = useAuth();
    const [searchParams] = useSearchParams();

    useEffect(() => {
        const accessToken = searchParams.get("accessToken");
        const refreshToken = searchParams.get("refreshToken");

        if (accessToken && refreshToken) {
            localStorage.setItem("accessToken", accessToken);
            localStorage.setItem("refreshToken", refreshToken);

            reload().then(() => {
                navigate("/home");
            });
        } else {
            console.error("Tokens not found in URL");
            navigate("/login-error");
        }
    }, [searchParams, navigate, reload]);

    return (
        <div className="flex items-center justify-center h-screen">
            <p>We are completing the login, please wait...</p>
        </div>
    );
};

export default GoogleOauthSuccess;