import { ExecutionResult } from "@/models/executionResult";
import { Card } from "../ui/card";
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from "../ui/dialog";
import { useState } from "react";
import { Button } from "../ui/button";
import { Clock, MemoryStick } from "lucide-react";

interface Props {
    executionResult: ExecutionResult;
    className?: string;
}

export function CodeVerificationResult({ executionResult, className }: Props) {
    const { time, memory, compileOutput, status, stdout } = executionResult;
    const [showModal, setShowModal] = useState(false);
    return (
        <>
            <Card className={`p-4 space-y-4 text-sm ${className}`}>
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                    <div>
                        <span className="flex items-center gap-2 font-medium"><Clock className="h-5 w-5" /> Time:</span>
                        <div>{time ?? "-"}</div>
                    </div>
                    <div>
                        <span className="flex items-center gap-2 font-medium"><MemoryStick className="h-5 w-5" /> Memory:</span>
                        <div>{memory ?? "-"}</div>
                    </div>
                </div>

                {compileOutput && (
                    <Button onClick={() => setShowModal(true)} className="btn-yellow btn-animation">
                        Compilation errors
                    </Button>
                )}
            </Card>
            <Dialog open={showModal} onOpenChange={(val) => !val && setShowModal(false)}>
                <DialogContent className="w-full sm:max-w-md max-h-[90vh] overflow-y-auto border">
                    <DialogHeader>
                        <DialogTitle className="text-center">🛠️ Compilation errors</DialogTitle>
                    </DialogHeader>
                    <div className="gap-4">
                        <div>
                            <span className="font-medium">Status: {status ?? "-"}</span>
                        </div>
                        <div>
                            <span className="font-medium">Out:</span>
                            <div>{stdout?.join(' ') ?? "-"}</div>
                        </div>
                    </div>
                    <div>
                        <pre className="mt-1 whitespace-pre-wrap break-words rounded-md border p-2 text-xs bg-gray">
                            {compileOutput}
                        </pre>
                    </div>
                </DialogContent>
                <DialogDescription className="font-mono"></DialogDescription>
            </Dialog>
        </>
    );
}

export default CodeVerificationResult;
