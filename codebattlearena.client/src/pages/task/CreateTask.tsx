import { useState } from "react";
import InlineNotification from "@/components/common/InlineNotification";
import TaskForm from "@/components/forms/TaskForm";


export function CreateTask() {
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
                            Create Task
                        </h1>
                    </div>
                    <TaskForm submitLabel="Created" onClose={handleMessageSaccess}></TaskForm>
                </div>
            </div>
      </>
  );
}

export default CreateTask;