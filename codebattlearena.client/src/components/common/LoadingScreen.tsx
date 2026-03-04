import { TerminalSquare } from "lucide-react";

export default function LoadingScreen() {
    return (
        <div className="flex h-auto items-center justify-center">
            <div className="text-center space-y-4 animate-fade-in">
                <div className="flex items-center justify-center space-x-2 animate-pulse">
                    <TerminalSquare className="w-6 h-6 text-primary" />
                    <span className="text-lg tracking-widest text-primary font-mono uppercase">Booting system...</span>
                </div>

                <div className="text-sm text-zinc-400 font-mono">
                    <p>Loading CodeBattleArea</p>
                </div>

                <div className="mt-4">
                    <span className="inline-block w-4 h-4 bg-primary rounded-full animate-ping"></span>
                </div>
            </div>
        </div>
    );
}
