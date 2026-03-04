import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogFooter,
    DialogClose,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";

import { useState } from "react";
import { Role } from "@/models/dbModels";
import { Label } from "../ui/label";

interface Props {
    open: boolean;
    onClose: () => void;
    defaultSelected?: string[];
    onSelect: (roles: string[]) => void;
}

export function RoleSelectModal({
    open,
    onClose,
    defaultSelected = [],
    onSelect,
}: Props) {
    const [selectedRoles, setSelectedRoles] = useState<string[]>(defaultSelected);

    const handleToggle = (role: string) => {
        setSelectedRoles((prev) =>
            prev.includes(role)
                ? prev.filter((r) => r !== role)
                : [...prev, role]
        );
    };

    const handleConfirm = () => {
        onSelect(selectedRoles);
    };

    return (
        <Dialog open={open} onOpenChange={(val) => !val && onClose()}>
            <DialogContent>
                <DialogHeader>
                    <DialogTitle>Select Roles</DialogTitle>
                </DialogHeader>

                <div className="grid gap-3 py-4">
                    {Object.values(Role).map((role) => (
                        <div key={role} className="flex items-center gap-2">
                            <Checkbox
                                id={role}
                                checked={selectedRoles.includes(role)}
                                onCheckedChange={() => handleToggle(role)}
                            />
                            <Label htmlFor={role} className="text-sm">
                                {role}
                            </Label>
                        </div>
                    ))}
                </div>

                <DialogFooter className="flex gap-2">
                    <DialogClose asChild>
                        <Button variant="outline" onClick={onClose} className="btn-animation">
                            Cancel
                        </Button>
                    </DialogClose>
                    <DialogClose asChild>
                        <Button className="btn-animation" onClick={handleConfirm}>Save</Button>
                    </DialogClose>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
}

export default RoleSelectModal;