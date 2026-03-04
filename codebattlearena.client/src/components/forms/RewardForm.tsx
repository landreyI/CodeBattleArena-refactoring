import { z } from "zod";
import { useForm, useWatch } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

import {
    Form,
    FormControl,
    FormField,
    FormItem,
    FormLabel,
    FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Reward } from "@/models/dbModels";
import LoadingScreen from "../common/LoadingScreen";
import ErrorMessage from "../common/ErrorMessage";
import { useState } from "react";
import { useItems } from "@/hooks/item/useItems";
import ItemMiniCard from "../cards/ItemMiniCard";
import { useCreateReward } from "@/hooks/quest/useCreateReward";
import { useUpdateReward } from "@/hooks/quest/useUpdateReward";
import ItemsSelectModal from "../modals/ItemsSelectModal";

export const formSchema = z.object({
    rewardType: z.string().min(3, { message: "Task name must be at least 3 characters." }),
    amount: z.preprocess(
        (val) => (val === "" || val == null ? undefined : Number(val)),
        z.number().min(1, { message: "Minimum 1" }).optional()
    ),
    itemId: z.preprocess(
        (val) => (val === "" || val == null ? undefined : Number(val)),
        z.number().min(1, { message: "Minimum 1" }).optional()
    ),
});

interface Props {
    reward?: Reward;
    onClose?: () => void;
    onUpdate?: (reward: Reward) => void;
    submitLabel?: string;
}

export function RewardForm({ reward, onClose, onUpdate, submitLabel = "Submit" }: Props) {
    const { items, loading: itemsLoad, error: itemsError } = useItems();
    const { createReward, loading: createIsLoad, error: createError } = useCreateReward();
    const { updateReward, loading: updateIsLoad, error: updateError } = useUpdateReward();
    const [openModal, setOpenModal] = useState(false);



    const openItemsModal = () => setOpenModal(true);
    const closeItemsModal = () => setOpenModal(false);

    const isEditing = !!reward;
    const isLoading = isEditing ? updateIsLoad : createIsLoad;
    const error = isEditing ? updateError : createError;

    const form = useForm<z.infer<typeof formSchema>>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            amount: reward?.amount ?? undefined,
            rewardType: reward?.rewardType,
            itemId: reward?.itemId ?? undefined,
        },
    });

    const itemId = useWatch({ control: form.control, name: "itemId" });

    const selectedItem = items.find(i => i.idItem === itemId);

    const handleSaveSelected = (itemId: number | null) => {
        form.setValue("itemId", itemId ?? undefined);
        closeItemsModal();
    };

    const buildReward = (values: z.infer<typeof formSchema>, reward?: Reward): Reward => ({
        idReward: reward?.idReward ?? null,
        item: null,
        amount: values.amount ?? null,
        rewardType: values.rewardType,
        itemId: values.itemId ?? null,
    });

    const onSubmit = async (values: z.infer<typeof formSchema>) => {
        try {
            const rewardData = buildReward(values, reward);
            const data = await (isEditing ? updateReward(rewardData) : createReward(rewardData));
            if (!isEditing && !isNaN(Number(data)))
                rewardData.idReward = Number(data);

            rewardData.item = items.find(item => item.idItem === values.itemId) ?? null;
            onUpdate?.(rewardData);
            onClose?.();
        } catch (err) {
            form.setError("root", {
                type: "manual",
                message: error?.message,
            });
        }
    };

    if (itemsLoad) return <LoadingScreen />;
    if (itemsError) return <ErrorMessage error={itemsError} />;

    return (
        <>
            <Form {...form}>
                <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
                    {form.formState.errors.root && (
                        <div className="bg-red-100 text-red-700 border border-red-300 p-3 rounded-md">
                            {form.formState.errors.root.message}
                        </div>
                    )}

                    <FormField
                        control={form.control}
                        name="rewardType"
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel>Reward Type</FormLabel>
                                <FormControl>
                                    <Input placeholder="Enter reward type" {...field} />
                                </FormControl>
                                <FormMessage />
                            </FormItem>
                        )}
                    />

                    <FormField
                        control={form.control}
                        name="amount"
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel>Amount</FormLabel>
                                <FormControl>
                                    <Input placeholder="Enter amount" type="number" {...field} />
                                </FormControl>
                                <FormMessage />
                            </FormItem>
                        )}
                    />

                    <div>
                        <div className="flex justify-between items-center mb-2">
                            <h3 className="text-md font-medium">Item (Optional)</h3>
                            <Button type="button" onClick={openItemsModal} className="btn-animation btn-primary">
                                Select Item
                            </Button>
                        </div>

                        <div className="relative w-fit">
                            {selectedItem && <ItemMiniCard item={selectedItem} />}
                        </div>

                    </div>

                    <Button type="submit" disabled={isLoading} className="w-full md:w-fit btn-animation">
                        {isLoading ? "Saving..." : submitLabel}
                    </Button>
                </form>
            </Form>
            <ItemsSelectModal open={openModal} onClose={closeItemsModal} items={items} onSaveSelect={handleSaveSelected} />
        </>
    );
}

export default RewardForm;
