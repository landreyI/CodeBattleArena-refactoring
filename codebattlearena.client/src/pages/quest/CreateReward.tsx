import { useState } from "react";
import InlineNotification from "@/components/common/InlineNotification";
import RewardForm from "@/components/forms/RewardForm";

export function CreateReward() {
    const [notification, setNotification] = useState<string | null>(null);

    const handleMessageSaccess = () => {
        setNotification(null);
        setNotification("Success");
    }

    return (
        <>
            {notification && (
                <InlineNotification message={notification} className="bg-blue" />
            )}

            <div className="glow-box">
                <div className="md:w-[50vw] sm:w-full mx-auto">
                    <div className="flex items-center justify-between mb-6">
                        <h1 className="text-4xl font-bold text-green-400 font-mono">
                            Create Reward
                        </h1>
                    </div>
                    <RewardForm submitLabel="Created" onClose={handleMessageSaccess}></RewardForm>
                </div>
            </div>
        </>
    );
}

export default CreateReward;