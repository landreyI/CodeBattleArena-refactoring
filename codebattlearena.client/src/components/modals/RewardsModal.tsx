import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
} from "@/components/ui/dialog";
import { Reward } from "@/models/dbModels";
import { useEffect, useState } from "react";
import { Button } from "../ui/button";
import { Checkbox } from "../ui/checkbox";
import RewardsList from "../lists/RewardsList";
import RewardForm from "../forms/RewardForm";
import EditModal from "./EditModal";

interface Props {
    open: boolean;
    rewards?: Reward[];
    taskPlayerRewards?: Reward[];
    onSaveSelected?: (updatedRewards: Reward[]) => void;
    onAddReward?: (addReward: Reward) => void;
    onClose: () => void;
}

export function RewardsModal({
    open,
    rewards = [],
    taskPlayerRewards = [],
    onSaveSelected,
    onAddReward,
    onClose,
}: Props) {
    const [taskPlayRewardsTemp, setTaskPlayRewardsTemp] = useState<Reward[]>([]);
    const [openRewardModal, setOpenRewardModal] = useState(false);

    useEffect(() => {
        setTaskPlayRewardsTemp(taskPlayerRewards);
    }, [taskPlayerRewards])

    const handleSelect = (idReward?: number) => {
        const selectedReward = rewards.find((r) => r.idReward === idReward);
        if (selectedReward && !taskPlayRewardsTemp.includes(selectedReward)) {
            setTaskPlayRewardsTemp((prev) => [...prev, selectedReward]);
        }
    };

    const handleDelete = (idReward?: number) => {
        setTaskPlayRewardsTemp((prev) => prev.filter((reward) => reward.idReward !== idReward));
    };
    return (
        <Dialog open={open} onOpenChange={(val) => !val && onClose()}>
            <DialogContent className="w-full sm:max-w-[75vw] max-h-[90vh] overflow-y-auto pb-0">
                <DialogHeader>
                    <DialogTitle>Manage Rewards</DialogTitle>
                </DialogHeader>

                <RewardsList
                    rewards={rewards}
                    className="max-h-[65vh] overflow-y-auto"
                    cardWrapperClassName="hover:scale-[1.02] transition relative h-full"
                    renderItemAddon={(reward) => {
                        const isSelected = taskPlayRewardsTemp.some(r => r.idReward === reward.idReward);
                        return (
                            <Checkbox
                                className="h-7 w-7"
                                checked={isSelected}
                                onCheckedChange={(checked) => {
                                    if (checked) handleSelect(reward.idReward ?? undefined);
                                    else handleDelete(reward.idReward ?? undefined);
                                }}
                            />
                        );
                    }}
                />

                <div className="flex flex-col md:flex-row justify-between mt-1 gap-3">
                    <div>
                        <Button
                            onClick={() => setOpenRewardModal(true)}
                            className="btn-animation btn-primary"
                        >
                            Add Reward
                        </Button>
                    </div>

                    <div className="flex gap-3">
                        <Button variant="outline" onClick={onClose} className="btn-animation">
                            Cancel
                        </Button>
                        <Button onClick={() => onSaveSelected?.(taskPlayRewardsTemp)} className="btn-animation btn-primary">
                            Save
                        </Button>
                    </div>
                </div>

                <EditModal open={openRewardModal} title="Create Reward" onClose={() => setOpenRewardModal(false)}>
                    <RewardForm onClose={() => setOpenRewardModal(false)} onUpdate={onAddReward} submitLabel="Create"></RewardForm>
                </EditModal>
            </DialogContent>
        </Dialog>
    );
}

export default RewardsModal;
