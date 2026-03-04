import { handleGoogleLogin } from "@/services/google";
import { Button } from "../components/ui/button";

export const LoginError = () => {
    return (
        <div className="flex items-center justify-center bg-card rounded-xl">
            <div className="p-8 rounded-lg shadow-lg max-w-md w-full">
                <h2 className="text-3xl font-semibold text-red mb-4">Login Failed</h2>
                <p className="text-foregraund text-lg mb-6">
                    Something went wrong while trying to log you in. Please try again later.
                </p>
                <Button
                    onClick={handleGoogleLogin}
                    className="btn-animation btn-red w-full"
                >
                    Try Again
                </Button>
            </div>
        </div>
    );
}

export default LoginError;
