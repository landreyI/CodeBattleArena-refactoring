import ItemForm from "@/components/forms/ItemForm";
import { useState } from "react";
import InlineNotification from "@/components/common/InlineNotification";


export function CreateItem() {
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
                <div className="md:w-[30vw] sm:w-full mx-auto">
                    <div className="flex items-center justify-between mb-6">
                        <h1 className="text-4xl font-bold text-green-400 font-mono">
                            Create Item
                        </h1>
                    </div>
                    <ItemForm submitLabel="Created" onClose={handleMessageSaccess}></ItemForm>
                </div>
            </div>
      </>
  );
}

export default CreateItem;