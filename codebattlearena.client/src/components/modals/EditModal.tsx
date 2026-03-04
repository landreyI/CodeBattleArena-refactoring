import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogFooter,
    DialogDescription,
} from "@/components/ui/dialog";
interface Props {
    open: boolean;
    title: string;
    onClose: () => void;
    children: React.ReactNode;
    classNameContent?: string;
}
export function EditModal({
    open,
    title,
    onClose,
    children,
    classNameContent = "w-full md:max-w-[70vw] max-h-[90vh] border border-2 p-0 rounded-3xl",
}: Props) {
    return (
        <Dialog open={open} onOpenChange={(val) => !val && onClose()}>
            <DialogContent className={`${classNameContent}`}>
                <DialogHeader className="bg-primary p-5 rounded-t-3xl border-b-2">
                    <DialogTitle>
                        <span className="text-background">{title}</span>
                    </DialogTitle>
                </DialogHeader>

                <div className={`p-5 md:max-w-[70vw] max-h-[70vh] overflow-y-auto`}>
                    {children}
                </div>

                <DialogFooter>
                    <DialogDescription className="font-mono" />
                </DialogFooter>
            </DialogContent>
        </Dialog>
    )
}

export default EditModal;