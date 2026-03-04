import { useNavigate, useParams } from "react-router-dom";
import { useTaskPlay } from "@/hooks/quest/useTaskPlay";
import { usePlayerProgress } from "@/hooks/quest/usePlayerProgress";
import { useAuth } from "@/contexts/AuthContext";
import EmptyState from "@/components/common/EmptyState";
import ErrorMessage from "@/components/common/ErrorMessage";
import LoadingScreen from "@/components/common/LoadingScreen";
import { getTaskParamPrimary, isEditRole } from "@/untils/businessRules";
import SettingMenu from "@/components/menu/SettingMenu";
import { useState } from "react";
import { TaskPlay } from "@/models/dbModels";
import { useTaskPlayRewards } from "@/hooks/quest/useTaskPlayRewards";
import QuestCard from "@/components/cards/QuestCard";
import { useDeleteTaskPlay } from "@/hooks/quest/useDeleteTaskPlay";
import InlineNotification from "@/components/common/InlineNotification";
import ProgressQuest from "@/components/common/ProgressQuest";
import RewardsList from "@/components/lists/RewardsList";
import TaskPlayParamsList from "@/components/lists/TaskPlayParamsList";
import { Label } from "@/components/ui/label";
import { Button } from "@/components/ui/button";
import { useClaimReward } from "@/hooks/quest/useClaimReward";
import { Badge } from "@/components/ui/badge";
import QuestResetTimer from "@/components/common/QuestResetTimer";
import { isQuestResetAvailable } from "@/untils/helpers";
import { showAnimationRotateIcons } from "@/components/animations/AnimationRotateIcons";
import { Coins, Star } from "lucide-react";
import EditModal from "@/components/modals/EditModal";
import { QuestForm } from "@/components/forms/QuestForm";

export function QuestPage() {
    const { taskPlayId } = useParams<{ taskPlayId: string }>();
    const { taskPlay, setTaskPlay, loading: taskPlayLoad, error: taskPlayError } = useTaskPlay(Number(taskPlayId));
    const { taskPlayRewards, reloadTaskPlayRewards } = useTaskPlayRewards(Number(taskPlayId));
    const { deleteTaskPlay, error: deleteTaskPlayError, loading: deleteTaskPlayLoad } = useDeleteTaskPlay();
    const { claimReward, loading: getRewaedLoad, error: getRewardError } = useClaimReward();

    const [notification, setNotification] = useState<string | null>(null);

    const { user, reload } = useAuth();
    const { playerProgress, setPlayerProgress, reloadProgress } = usePlayerProgress(user?.id, Number(taskPlayId));

    const navigate = useNavigate();
    const [showEditQuest, setShowEditQuest] = useState(false);

    const handleUpdateQuest = (updatedTaskPlay: TaskPlay) => {
        setTaskPlay(updatedTaskPlay);
        reloadTaskPlayRewards();
    };

    const handleDeletQuest = async () => {
        const success = await deleteTaskPlay(Number(taskPlayId));
        if(success)
            navigate(`/quest/list-quests`);
    }

    const handleGetReward = async () => {
        const success = await claimReward(user?.id, taskPlay?.idTask ?? undefined);
        if (success) {
            setNotification(null);
            setNotification("Award received");
            if (playerProgress) {
                setPlayerProgress({
                    ...playerProgress,
                    isGet: true,
                });
            }
            reload();
            await showAnimationRotateIcons({ icon: <Coins className="text-yellow" /> });
            await showAnimationRotateIcons({ icon: <Star className="text-purple" /> });
        }
    }

    if (taskPlayLoad || deleteTaskPlayLoad || getRewaedLoad) return <LoadingScreen />
    if (taskPlayError) return <ErrorMessage error={taskPlayError} />;
    if (!taskPlay) return <EmptyState message="Session not found" />;

    const error = deleteTaskPlayError || getRewardError;
    const isEdit = isEditRole(user?.roles ?? []);
    const isQuestReset = isQuestResetAvailable(playerProgress?.completedAt ?? undefined, taskPlay?.repeatAfterDays ?? undefined);

    return (
        <>
            {error && <InlineNotification message={error.message} className="bg-red" />}

            {notification && (
                <InlineNotification message={notification} className="bg-blue" />
            )}

            <div className="glow-box">
                <div className="md:w-[45vw] sm:w-[100vw] mx-auto">
                    <div className="flex items-center justify-between mb-6">
                        <h1 className="text-4xl font-bold text-primary font-mono">
                            Quest - details
                        </h1>
                        {isEdit && (
                            <SettingMenu
                                setShowEdit={setShowEditQuest}
                                handleDelet={handleDeletQuest}
                            />
                        )}
                    </div>

                    <ProgressQuest
                        playerProgress={playerProgress}
                        taskPlayParamValue={getTaskParamPrimary(taskPlay)}
                        className="w-fit"
                    />

                    <QuestCard taskPlay={taskPlay} className="mb-2"/>

                    {playerProgress?.isCompleted && !playerProgress?.isGet ? (
                        <Button
                            className="btn-animation"
                            onClick={handleGetReward}
                        >
                            Claim
                        </Button>
                    ) : playerProgress?.isGet ? (
                        <div className="flex flex-col md:flex-row gap-2">
                                <Badge className="text-sm">Award Received</Badge>
                                <QuestResetTimer
                                    completedAt={playerProgress?.completedAt ?? undefined}
                                    repeatAfterDays={taskPlay?.repeatAfterDays ?? undefined}
                                />
                                {isQuestReset && (
                                    <Button
                                        className="btn-animation"
                                        onClick={reloadProgress}
                                    >
                                        Reload quest
                                    </Button>
                                )}
                        </div>

                    ) : null}

                    {isEdit && (
                        <>
                            <Label className="mt-5 mb-1 text-base font-semibold text-primary">Options</Label>
                            <TaskPlayParamsList params={taskPlay?.taskPlayParams ?? []} cardWrapperClassName="" />
                        </>
                    )}

                    {taskPlayRewards && taskPlayRewards.length != 0 && (
                        <>
                            <Label className="mt-5 mb-1 text-base font-semibold text-primary">Rewards</Label>
                            <RewardsList
                                rewards={taskPlayRewards}
                                className="gap-4"
                                columns="sm:grid-cols-2 md:grid-cols-2"
                                cardWrapperClassName="hover:scale-[1.02] transition relative h-full"
                            />
                        </>
                    )}

                </div>
            </div>
            {taskPlay && (
                <EditModal open={showEditQuest} title="Edit Quest" onClose={() => setShowEditQuest(false)}>
                    <QuestForm taskPlay={taskPlay} onClose={() => setShowEditQuest(false)} onUpdate={handleUpdateQuest} submitLabel="Save"></QuestForm>
                </EditModal>
            )}
        </>
    )
}

export default QuestPage;