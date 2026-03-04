import { useState } from "react";
import { GenericDropdownMenu, MenuAction } from "./GenericDropdownMenu";
import { AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle, AlertDialogTrigger } from "@/components/ui/alert-dialog"; // из shadcn/ui
import { Settings } from "lucide-react";

interface Props {
    setShowEdit?: (e: any) => void;
    handleDelet?: () => void;
    actionsProp?: MenuAction[];
}


export function SettingMenu({ setShowEdit, handleDelet, actionsProp }: Props) {
    const [showConfirm, setShowConfirm] = useState(false);

    const actions: MenuAction[] = [
        ...(setShowEdit
            ? [{ label: "Edit", onClick: setShowEdit, shortcut: "⌘E" }]
            : []),
        ...(handleDelet
            ? [{
                label: "Delete",
                onClick: () => setShowConfirm(true), // не сразу удаляет
                shortcut: "⨉D"
            }]
            : []),
        ...(actionsProp ?? [])
    ];

    return (
        <>
            <GenericDropdownMenu
                triggerContent={
                    <button className="hover:text-primary transition-colors" aria-label="Settings">
                        <Settings/>
                    </button>
                }
                menuLabel="Setting"
                actions={actions}
            />

            {/* модалка подтверждения */}
            <AlertDialog open={showConfirm} onOpenChange={setShowConfirm}>
                <AlertDialogContent className="w-[90vw] md:w-fit border border-2 p-0 rounded-3xl">
                    <AlertDialogHeader className="bg-primary p-3 rounded-t-3xl border-b-2">
                        <AlertDialogTitle>
                            <span className="text-background">Are you sure you want to delete?</span>
                        </AlertDialogTitle>
                    </AlertDialogHeader>
                    <AlertDialogFooter className="p-5">
                        <AlertDialogCancel className="btn-animation">Cancel</AlertDialogCancel>
                        <AlertDialogAction
                            className="btn-animation"
                            onClick={() => {
                                handleDelet?.();
                                setShowConfirm(false);
                            }}
                        >
                            Confirm
                        </AlertDialogAction>
                    </AlertDialogFooter>
                </AlertDialogContent>
            </AlertDialog>
        </>
    );
}

export default SettingMenu;
