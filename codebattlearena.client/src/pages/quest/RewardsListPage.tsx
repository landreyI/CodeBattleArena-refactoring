import EmptyState from "@/components/common/EmptyState";
import { useRewards } from "@/hooks/quest/useRewards";
import RewardsList from "@/components/lists/RewardsList";
import SettingMenu from "@/components/menu/SettingMenu";
import { Reward } from "@/models/dbModels";
import ErrorMessage from "@/components/common/ErrorMessage";
import LoadingScreen from "@/components/common/LoadingScreen";
import { useDeleteReward } from "@/hooks/quest/useDeleteReward";
import InlineNotification from "@/components/common/InlineNotification";
import { useState } from "react";
import EditModal from "@/components/modals/EditModal";
import RewardForm from "@/components/forms/RewardForm";

export function RewardsListPage() {
    const { rewards, setRewards, loading: rewardLoad, error: rewardError, reloadRewards } = useRewards();
    const { deleteReward, error: deleteRewardError } = useDeleteReward();

    const [editingRewardId, setEditingRewardId] = useState<number | null>(null);

    const handleUpdateReward = (updatedReward: Reward) => {
        setRewards((prevRewards) =>
            prevRewards.map((reward) =>
                reward.idReward === updatedReward.idReward ? updatedReward : reward
            )
        )
    };

    const handleDeletReward = async (idReward?: number) => {
        const success = await deleteReward(idReward);
        if (success)
            setRewards((prevRewards) => prevRewards.filter((reward) => reward.idReward !== idReward))
    }

    if (rewardLoad) return <LoadingScreen />
    if (rewardError) return <ErrorMessage error={rewardError} />;

    const error = deleteRewardError;

    return (
        <>
            {error && <InlineNotification message={error.message} className="bg-red" />}
            {!rewards || rewards.length === 0 && (<EmptyState message="Rewards not found" />)}

            <RewardsList
                rewards={rewards}
                cardWrapperClassName="hover:scale-[1.02] transition h-full"
                renderItemAddon={(reward) => (
                    <div className="flex justify-end">
                        <SettingMenu
                            setShowEdit={() => setEditingRewardId(reward.idReward ?? null)}
                            handleDelet={() => handleDeletReward(reward.idReward ?? undefined)}
                        />
                        <EditModal
                            open={editingRewardId === reward.idReward}
                            title="Edit Reward"
                            onClose={() => setEditingRewardId(null)}
                        >
                            <RewardForm reward={reward} onClose={() => setEditingRewardId(null)} onUpdate={handleUpdateReward} submitLabel="Save"></RewardForm>
                        </EditModal>
                    </div>
                )}
            />
        </>
    );
}

export default RewardsListPage;