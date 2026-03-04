import { z } from "zod";
import { useForm } from "react-hook-form";
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
import { Reward, TaskParamKey, TaskPlay, TaskPlayParam, TaskType } from "@/models/dbModels";
import LoadingScreen from "../common/LoadingScreen";
import ErrorMessage from "../common/ErrorMessage";
import { Textarea } from "../ui/textarea";
import { useEffect, useState } from "react";
import { useTaskPlayRewards } from "@/hooks/quest/useTaskPlayRewards";
import { Switch } from "../ui/switch";
import RewardsModal from "../modals/RewardsModal";
import { useRewards } from "@/hooks/quest/useRewards";
import { useCreateTaskPlay } from "@/hooks/quest/useCreateTaskPlay";
import { useUpdateTaskPlay } from "@/hooks/quest/useUpdateTaskPlay";
import RewardCard from "../cards/RewardCard";
import RewardsList from "../lists/RewardsList";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "../ui/select";
import { RadioGroup, RadioGroupItem } from "../ui/radio-group";
import { Label } from "../ui/label";

// Определяем схему валидации формы
const paramSchema = z.object({
    paramKey: z.string().min(3, { message: "Param key must be at least 3 characters." }),
    paramValue: z.string().min(1, { message: "Param value must be at least 1 character." }),
    isPrimary: z.boolean(),
});

export const formSchema = z
    .object({
        name: z.string().min(3, { message: "Task name must be at least 3 characters." }),
        description: z.string().min(10, { message: "Description must be at least 10 characters." }),
        type: z.string().min(5, { message: "Type must be at least 5 characters." }),
        experience: z.preprocess(
            (val) => (val === "" || val == null ? undefined : Number(val)),
            z.number().min(1, { message: "Minimum 1" }).optional()
        ),
        rewardCoin: z.preprocess(
            (val) => (val === "" || val == null ? undefined : Number(val)),
            z.number().min(1, { message: "Minimum 1" }).optional()
        ),
        isRepeatable: z.boolean(),
        repeatAfterDays: z.preprocess(
            (val) => (val === "" || val == null ? undefined : Number(val)),
            z.number().min(1, { message: "Minimum 1" }).optional()
        ),
        idRewards: z.array(z.number()).optional(),
        taskPlayParams: z
            .array(paramSchema)
            .refine(
                (params) => params.filter((p) => p.isPrimary).length <= 1,
                { message: "Only one parameter can be primary." }
            ),
    })

interface Props {
    taskPlay?: TaskPlay;
    onClose?: () => void;
    onUpdate?: (taskPlay: TaskPlay) => void;
    submitLabel?: string;
}

export function QuestForm({ taskPlay, onClose, onUpdate, submitLabel }: Props) {
    const { taskPlayRewards, setTaskPlayRewards, loading: taskPlayRewardsLoad,
        error: taskPlayRewardsError, reloadTaskPlayRewards } = useTaskPlayRewards(taskPlay?.idTask ?? undefined);
    const { rewards, setRewards, loading: rewardLoad, error: rewardError, reloadRewards } = useRewards();

    const { createTaskPlay, loading: createIsLoad, error: createError } = useCreateTaskPlay();
    const { updateTaskPlay, loading: updateIsLoad, error: updateError } = useUpdateTaskPlay();

    const [openRewardsModal, setOpenRewardsModal] = useState(false);
    const [selectedIdRewards, setSelectedIdRewards] = useState<number[]>([]);

    useEffect(() => {
        setSelectedIdRewards(
            taskPlayRewards
                .map(r => r.idReward)
                .filter((id): id is number => id != null)
        );
    }, [taskPlayRewards])

    const handleSaveSelected = (updatedRewards: Reward[]) => {
        // Добавляем выбранные данные в форму
        setTaskPlayRewards(updatedRewards);
        setSelectedIdRewards(
            updatedRewards
                .map(r => r.idReward)
                .filter((id): id is number => id != null)
        );

        setOpenRewardsModal(false);
    };

    const handleAddReward = (addReward: Reward) => {
        setRewards((prevRewards) => [...prevRewards, addReward]);
    };



    // Подменяем в зависимости от контекста (создание или редактирование)
    const isEditing = !!taskPlay;

    const isLoading = isEditing ? updateIsLoad : createIsLoad;
    const error = isEditing ? updateError : createError;

    const form = useForm<z.infer<typeof formSchema>>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            name: taskPlay?.name || "",
            description: taskPlay?.description || "",
            type: taskPlay?.type || "",
            experience: taskPlay?.experience ?? undefined,
            rewardCoin: taskPlay?.rewardCoin ?? undefined,
            isRepeatable: taskPlay?.isRepeatable ?? false,
            repeatAfterDays: taskPlay?.repeatAfterDays ?? undefined,
            idRewards: Array.isArray(taskPlayRewards)
                ? taskPlayRewards
                    .map(data => data.idReward)
                    .filter((id): id is number => id !== undefined && id !== null)
                : [],
            taskPlayParams: taskPlay?.taskPlayParams ?? [{ paramKey: "", paramValue: "", isPrimary: false }],
        },
    });

    if (taskPlayRewardsLoad || rewardLoad) return <LoadingScreen />

    const errorHook = taskPlayRewardsError || rewardError;
    if (errorHook) return <ErrorMessage error={errorHook} />;

    function buildTaskPlayData(values: z.infer<typeof formSchema>, taskPlay?: TaskPlay): TaskPlay {
        return {
            ...(taskPlay ?? {}),
            name: values.name,
            description: values.description,
            type: values.type,
            experience: values.experience ?? null,
            rewardCoin: values.rewardCoin ?? null,
            isRepeatable: values.isRepeatable,
            repeatAfterDays: values.repeatAfterDays,
            taskPlayParams: values.taskPlayParams ?? [],
        };
    }

    const onSubmit = async (values: z.infer<typeof formSchema>) => {
        try {
            let result = null;

            const taskData = buildTaskPlayData(values, taskPlay);
            result = await (taskPlay ? updateTaskPlay(taskData, selectedIdRewards)
                : createTaskPlay(taskData, selectedIdRewards));

            if (onUpdate && taskPlay)
                onUpdate(taskData);

            if (onClose != null)
                onClose();

        } catch (err) {
            console.error(err);

            const standardError = error;

            form.setError("root", {
                type: "manual",
                message: standardError?.message,
            });
        }
    };

    return (
        <>
            <Form {...form}>
                <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
                
                    {form.formState.errors.root && (
                        <div className="bg-red-50 text-red-600 border border-red-200 p-3 rounded">
                            {form.formState.errors.root.message}
                        </div>
                    )}

                    <FormField
                        control={form.control}
                        name="name"
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel>Quest Name</FormLabel>
                                <FormControl>
                                    <Input placeholder="Enter quest name" {...field} />
                                </FormControl>
                                <FormMessage />
                            </FormItem>
                        )}
                    />

                    <FormField
                        control={form.control}
                        name="description"
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel>Description</FormLabel>
                                <FormControl>
                                    <Textarea placeholder="Enter description" {...field} />
                                </FormControl>
                                <FormMessage />
                            </FormItem>
                        )}
                    />

                    <FormField
                        control={form.control}
                        name="type"
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel>Type</FormLabel>
                                <FormControl>
                                    <Select
                                        onValueChange={field.onChange}
                                        value={field.value}
                                        defaultValue={field.value}
                                    >
                                        <SelectTrigger>
                                            <SelectValue placeholder="Select Type" />
                                        </SelectTrigger>
                                        <SelectContent>
                                            {Object.values(TaskType).map((key) => (
                                                <SelectItem key={key} value={key}>
                                                    {key}
                                                </SelectItem>
                                            ))}
                                        </SelectContent>
                                    </Select>
                                </FormControl>
                                <FormMessage />
                            </FormItem>
                        )}
                    />

                    <FormField
                        control={form.control}
                        name="experience"
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel>Experience (Optional)</FormLabel>
                                <FormControl>
                                    <Input type="number" placeholder="Enter experience" {...field} />
                                </FormControl>
                                <FormMessage />
                            </FormItem>
                        )}
                    />

                    <FormField
                        control={form.control}
                        name="rewardCoin"
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel>Coin (Optional)</FormLabel>
                                <FormControl>
                                    <Input type="number" placeholder="Enter coin" {...field} />
                                </FormControl>
                                <FormMessage />
                            </FormItem>
                        )}
                    />

                    <FormField
                        control={form.control}
                        name="isRepeatable"
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel>Will there be a repetition</FormLabel>
                                <FormControl>
                                    <Switch
                                        checked={field.value}
                                        onCheckedChange={field.onChange}
                                    />
                                </FormControl>
                                <FormMessage />
                            </FormItem>
                        )}
                    />

                    <FormField
                        control={form.control}
                        name="repeatAfterDays"
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel>Repeat after days (Optional)</FormLabel>
                                <FormControl>
                                    <Input type="number" placeholder="Enter days" {...field} />
                                </FormControl>
                                <FormMessage />
                            </FormItem>
                        )}
                    />

                    <FormField
                        control={form.control}
                        name="taskPlayParams"
                        render={() => (
                            <FormItem>
                                <FormLabel>Options</FormLabel>
                                <RadioGroup
                                    onValueChange={(selectedIndex) => {
                                        const currentParams = form.getValues("taskPlayParams");
                                        const updatedParams = currentParams.map((param, i) => ({
                                            ...param,
                                            isPrimary: i === Number(selectedIndex),
                                        }));
                                        form.setValue("taskPlayParams", updatedParams);
                                    }}
                                    value={form
                                        .watch("taskPlayParams")
                                        .findIndex((param) => param.isPrimary)
                                        .toString()}
                                    className=""
                                >
                                    {form.watch("taskPlayParams").map((param, index) => (
                                        <div
                                            key={index}
                                            className="flex flex-col justify-between items-center md:flex-row gap-4 bg-muted p-2 rounded-xl"
                                        >
                                            <FormField
                                                control={form.control}
                                                name={`taskPlayParams.${index}.paramKey`}
                                                render={({ field }) => (
                                                    <FormItem className="flex-1 w-full">
                                                        <FormControl>
                                                            <Select
                                                                onValueChange={field.onChange}
                                                                value={field.value}
                                                                defaultValue={field.value}
                                                            >
                                                                <SelectTrigger>
                                                                    <SelectValue placeholder="Selec param key" />
                                                                </SelectTrigger>
                                                                <SelectContent>
                                                                    {Object.values(TaskParamKey).map((key) => (
                                                                        <SelectItem key={key} value={key}>
                                                                            {key}
                                                                        </SelectItem>
                                                                    ))}
                                                                </SelectContent>
                                                            </Select>
                                                        </FormControl>
                                                        <FormMessage />
                                                    </FormItem>
                                                )}
                                            />
                                            <FormField
                                                control={form.control}
                                                name={`taskPlayParams.${index}.paramValue`}
                                                render={({ field }) => (
                                                    <FormItem>
                                                        <FormControl>
                                                            <Input {...field} placeholder="Enter param value"/>
                                                        </FormControl>
                                                        <FormMessage />
                                                    </FormItem>
                                                )}
                                            />
                                            <div className="flex justify-center items-center">
                                                <RadioGroupItem
                                                    value={index.toString()}
                                                    id={`param-${index}`}
                                                    className="mt-2"
                                                />
                                                <Label htmlFor={`param-${index}`} className="ml-2 mt-2">
                                                    Primary
                                                </Label>
                                            </div>
                                            <Button
                                                type="button"
                                                variant="destructive"
                                                className="btn-animation btn-red"
                                                onClick={() => {
                                                    const current = form.getValues("taskPlayParams");
                                                    current.splice(index, 1);
                                                    form.setValue("taskPlayParams", [...current]);
                                                }}
                                            >
                                                Remove
                                            </Button>
                                        </div>
                                    ))}
                                </RadioGroup>
                                <FormMessage />
                            </FormItem>
                        )}
                    />

                    <div className="flex flex-col md:flex-row gap-3">
                        <Button
                            type="button"
                            className="btn-animation"
                            onClick={() =>
                                form.setValue("taskPlayParams", [
                                    ...form.getValues("taskPlayParams"),
                                    { paramKey: "", paramValue: "", isPrimary: false },
                                ])
                            }
                        >
                            Add Param
                        </Button>

                        <Button type="button" onClick={() => setOpenRewardsModal(true)} className="btn-animation btn-primary">
                            Select rewards
                        </Button>
                    </div>

                    <RewardsList
                        rewards={taskPlayRewards}
                        columns="grid-cols-1 sm:grid-cols-2 lg:grid-cols-3"
                        cardWrapperClassName="hover:scale-[1.02] transition relative h-full"
                    />

                    <Button type="submit" disabled={isLoading} className="w-full md:w-fit btn-animation">
                        {isLoading ? "Saving..." : submitLabel}
                    </Button>
                </form>
            </Form>

            <RewardsModal
                open={openRewardsModal}
                rewards={rewards}
                taskPlayerRewards={taskPlayRewards}
                onClose={() => setOpenRewardsModal(false)}
                onSaveSelected={handleSaveSelected}
                onAddReward={handleAddReward}
            />
        </>
    );
}

export default QuestForm;
