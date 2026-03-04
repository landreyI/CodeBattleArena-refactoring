import { Reward } from "@/models/dbModels";
import { motion } from "framer-motion";
import RewardCard from "../cards/RewardCard";
interface Props {
    rewards: Reward[];
    renderItemAddon?: (reward: Reward) => React.ReactNode;
    cardWrapperClassName?: string;
    className?: string;
    columns?: string;
}

export function RewardsList({ rewards, renderItemAddon, cardWrapperClassName, className, columns = "grid-cols-1 sm:grid-cols-2 md:grid-cols-3" }: Props) {

    return (
        <motion.div
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: 10 }}
            transition={{ duration: 0.3 }}
            className={`grid gap-3 ${className} ${columns}`}
        >
            {rewards.map((reward) => (
                <div key={reward.idReward} className="flex flex-col gap-2 p-3 border rounded-2xl">
                    <RewardCard
                        reward={reward}
                        className={cardWrapperClassName}
                    >
                    </RewardCard>
                    {renderItemAddon?.(reward)}
                </div>
            ))}
        </motion.div>
    );
}
export default RewardsList;