import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogFooter,
    DialogDescription,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button"
import { handleGoogleLogin } from "@/services/google";

interface Props {
    open: boolean;
    onClose: () => void;
}

export function AuthorizationModal({ open, onClose }: Props) {
    return (
        <Dialog open={open} onOpenChange={(val) => !val && onClose()}>
            <DialogContent className="w-full sm:max-w-md max-h-[90vh] overflow-y-auto border">
                <DialogHeader>
                    <DialogTitle className="text-center">Authorization</DialogTitle>
                </DialogHeader>

                <div className="mt-4 text-center">
                    <Button
                        variant="outline"
                        onClick={handleGoogleLogin}
                        className="border"
                    >
                        <img
                            src="https://developers.google.com/identity/images/g-logo.png"
                            alt="Google"
                            className="w-6 h-6 rounded-full p-1"
                        />
                        <span>Sign in with Google</span>
                    </Button>

                </div>

                <DialogFooter className="mt-4">
                    <p className="text-sm">
                        By signing in, you agree to our <span className="underline cursor-pointer">Terms</span>.
                    </p>
                    <DialogDescription className="font-mono"></DialogDescription>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
}

export default AuthorizationModal;
