import { TerminalSquare } from "lucide-react";

export default function ScreenBlocker({ open }: { open: boolean }) {
    if (!open) return null;

    return (
        <div
            className="fixed inset-0 z-[109] bg-black/60 flex items-center justify-center"
            style={{ pointerEvents: "all" }}
        >
            <div className="flex flex-col items-center justify-center space-y-4">
                <div className="flex items-center justify-center space-x-2 animate-pulse">
                    <TerminalSquare className="w-6 h-6 text-primary" />
                    <span className="text-lg tracking-widest text-primary font-mono uppercase">
                        Booting system...
                    </span>
                </div>

                <div className="text-sm text-zinc-400 font-mono text-center">
                    <p>Loading CodeBattleArea</p>
                </div>

                <div className="mt-4">
                    <span className="inline-block w-4 h-4 bg-primary rounded-full animate-ping"></span>
                </div>
            </div>
        </div>
    );
}