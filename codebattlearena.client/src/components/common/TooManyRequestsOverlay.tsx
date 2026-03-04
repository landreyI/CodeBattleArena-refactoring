import { Button } from "../ui/button";
import { Dialog, DialogContent } from "../ui/dialog";
import { AlertTriangle } from "lucide-react"; // или другая иконка

interface Props {
    onReload: () => void;
}

export default function TooManyRequestsOverlay({ onReload }: Props) {
    return (
        <Dialog defaultOpen>
            <DialogContent
                onInteractOutside={(e) => e.preventDefault()}
                className="w-[90vw] md:w-full max-w-lg mx-auto bg-red-100 border-0 border-l-8 border-red 
                           text-red-800 rounded-2xl shadow-xl p-6 animate-in zoom-in-75"
            >
                <div className="flex items-center gap-4">
                    <AlertTriangle className="w-10 h-10 text-red animate-pulse" />
                    <div>
                        <p className="text-3xl font-extrabold">429 Error</p>
                        <p className="text-lg font-semibold italic opacity-80">Too Many Requests... ALL THE TIME</p>
                    </div>
                </div>

                <p className="text-base leading-relaxed">
                    Whoa! Slow down there, turbo. You've sent too many requests in a short time.
                    Give it a few seconds, then hit the button below.
                </p>

                <div className="flex justify-end">
                    <Button
                        onClick={onReload}
                        className="btn-red btn-animation font-semibold"
                    >
                        Try Again
                    </Button>
                </div>
            </DialogContent>
        </Dialog>
    );
}
