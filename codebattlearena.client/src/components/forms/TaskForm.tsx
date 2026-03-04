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
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Difficulty, TaskInputData, TaskProgramming, InputData } from "@/models/dbModels";
import { useLangsProgramming } from "@/hooks/useLangsProgramming";
import LoadingScreen from "../common/LoadingScreen";
import ErrorMessage from "../common/ErrorMessage";
import EmptyState from "../common/EmptyState";
import { Textarea } from "../ui/textarea";
import { useCreateTask } from "@/hooks/task/useCreateTask";
import { useUpdateTask } from "@/hooks/task/useUpdateTask";
import { useFieldArray } from "react-hook-form";
import { useInputDatas } from "@/hooks/task/useInputDatas";
import { useState } from "react";
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTrigger } from "../ui/dialog";
import { Checkbox } from "../ui/checkbox";
import { getDifficultyColor } from "@/untils/helpers";

// Определяем схему валидации формы
export const formSchema = z
    .object({
        name: z.string().min(3, { message: "Task name must be at least 3 characters." }),
        idLangProgramming: z.number().min(0, { message: "Select language" }),
        difficulty: z.nativeEnum(Difficulty, {
            errorMap: () => ({ message: "Invalid difficulty" }),
        }),
        textTask: z.string().min(10, { message: "Description must be at least 10 characters." }),
        preparation: z.string().nonempty("Preparation is required"),
        verificationCode: z.string().nonempty("Verification code is required"),
        taskInputData: z.array(
            z.object({
                idTaskProgramming: z.number().nullable(),
                idInputDataTask: z.number().nullable(),
                inputData: z.object({
                    idInputData: z.number().nullable(),
                    data: z.string().min(1),
                }).nullable(),
                answer: z.string().min(1),
            })
        ).optional(),
    })

interface Props {
    task?: TaskProgramming;
    onClose?: () => void;
    onUpdate?: (updatedTask: TaskProgramming) => void;
    submitLabel?: string;
}

export function TaskForm({ task, onClose, onUpdate, submitLabel }: Props) {
    const { langsProgramming, loading: langsLoad, error: langsError } = useLangsProgramming();
    const { inputDatas, setInputDatas, loading: datasLoad, error: datasError, reloadInputDatas } = useInputDatas();
    const { createTask, loading: createIsLoad, error: createError } = useCreateTask();
    const { updateTask, loading: updateIsLoad, error: updateError } = useUpdateTask();

    const [openModal, setOpenModal] = useState(false);
    const [selectedInputDatas, setSelectedInputDatas] = useState<InputData[]>([]);

    // Открытие и закрытие модального окна
    const openInputDataModal = () => setOpenModal(true);
    const closeInputDataModal = () => setOpenModal(false);

    const handleSaveSelected = () => {
        // Добавляем выбранные данные в форму
        selectedInputDatas.forEach(inputData => {
            append({
                idTaskProgramming: null,
                idInputDataTask: inputData.idInputData,
                inputData: { idInputData: inputData.idInputData, data: inputData.data },
                answer: "", // Будет пустым, как указано в задаче
            });
        });

        closeInputDataModal();
    };


    // Подменяем в зависимости от контекста (создание или редактирование)
    const isEditing = !!task;

    const isLoading = isEditing ? updateIsLoad : createIsLoad;
    const error = isEditing ? updateError : createError;

    const form = useForm<z.infer<typeof formSchema>>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            name: task?.name || "",
            idLangProgramming: task?.langProgrammingId || -1,
            difficulty: task?.difficulty || Difficulty.Easy,
            textTask: task?.textTask || "",
            preparation: task?.preparation || "",
            verificationCode: task?.verificationCode || "",
            taskInputData: task?.taskInputData?.map(data => ({
                idTaskProgramming: data.idTaskProgramming ?? null,
                idInputDataTask: data.idInputDataTask ?? null,
                inputData: data.inputData ? {
                    idInputData: data.inputData.idInputData ?? null,
                    data: data.inputData.data,
                } : null,
                answer: data.answer,
            })) || [],
        },
    });

    const { fields, append, remove } = useFieldArray({
        control: form.control,
        name: "taskInputData",
    });

    if (langsLoad || datasLoad) return <LoadingScreen />
    if (langsError) return <ErrorMessage error={langsError} />;
    if (datasError) return <ErrorMessage error={datasError} />;
    if (!langsProgramming) return <EmptyState message="Langs programming not found" />;

    function buildTaskData(values: z.infer<typeof formSchema>, task?: TaskProgramming): TaskProgramming {
        return {
            idTaskProgramming: task?.idTaskProgramming ?? null,
            langProgramming: langsProgramming?.find(lang => lang.idLang === values.idLangProgramming) ?? null,
            taskInputData: values.taskInputData ?? [],

            name: values.name,
            langProgrammingId: values.idLangProgramming,
            difficulty: values.difficulty,
            textTask: values.textTask,
            preparation: values.preparation,
            verificationCode: values.verificationCode,
        };
    }

    const onSubmit = async (values: z.infer<typeof formSchema>) => {
        try {
            let result = null;

            const taskData = buildTaskData(values, task);
            result = await (task ? updateTask(taskData) : createTask(taskData));

            if (onUpdate && task)
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
                            <FormLabel>Task Name</FormLabel>
                            <FormControl>
                                <Input placeholder="Enter task name" {...field} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                <FormField
                    control={form.control}
                    name="idLangProgramming"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Programming Language</FormLabel>
                            <FormControl>
                                <Select
                                    onValueChange={(val) => field.onChange(Number(val))}
                                    value={field.value.toString()}
                                >
                                    <SelectTrigger>
                                        <SelectValue>
                                            {
                                                // Проверка: есть ли такое значение среди lang.idLang
                                                langsProgramming.some(lang => lang.idLang === field.value)
                                                    ? langsProgramming.find(lang => lang.idLang === field.value)?.nameLang
                                                    : "Select valid language"
                                            }
                                        </SelectValue>
                                    </SelectTrigger>
                                    <SelectContent>
                                        {langsProgramming.map((lang) => (
                                            <SelectItem key={lang.idLang} value={lang.idLang.toString()}>
                                                {lang.nameLang}
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
                    name="difficulty"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Difficulty</FormLabel>
                            <FormControl>
                                <Select onValueChange={field.onChange} value={field.value}>
                                    <SelectTrigger>
                                        <SelectValue placeholder="Select difficulty" />
                                    </SelectTrigger>
                                    <SelectContent>
                                        {Object.values(Difficulty).map((diff) => (
                                            <SelectItem key={diff} value={diff} className={`${getDifficultyColor(diff)} mt-1`}>
                                                {diff}
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
                    name="textTask"
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
                    name="preparation"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Preparation</FormLabel>
                            <FormControl>
                                <Textarea placeholder="Enter preparation notes" {...field} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                <FormField
                    control={form.control}
                    name="verificationCode"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Verification Code</FormLabel>
                            <FormControl>
                                <Textarea placeholder="Enter verification code" {...field} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                <h3 className="text-lg font-semibold mb-2">Test Cases</h3>

                <div>
                    {/* Модальное окно для выбора входных данных */}
                    <Dialog open={openModal} onOpenChange={setOpenModal}>
                        <DialogContent>
                            <DialogHeader>
                                <h3 className="text-lg font-semibold mb-2">Select Input Data</h3>
                            </DialogHeader>
                            <div className="space-y-2">
                                {inputDatas.map((inputData) => (
                                    <div key={inputData.idInputData} className="flex items-center gap-2">
                                        <Checkbox
                                            checked={selectedInputDatas.includes(inputData)}
                                            onCheckedChange={(checked) => {
                                                if (checked) {
                                                    setSelectedInputDatas((prev) => [...prev, inputData]);
                                                } else {
                                                    setSelectedInputDatas((prev) =>
                                                        prev.filter((item) => item.idInputData !== inputData.idInputData)
                                                    );
                                                }
                                            }}
                                        />
                                        <span>{inputData.data}</span>
                                    </div>
                                ))}
                            </div>
                            <DialogFooter>
                                <Button className="btn-animation" variant="outline" onClick={closeInputDataModal}>
                                    Cancel
                                </Button>
                                <Button className="btn-animation" onClick={handleSaveSelected}>Save</Button>
                            </DialogFooter>
                        </DialogContent>
                    </Dialog>

                    <div className="border rounded-xl mb-4 p-3 space-y-4">

                        {fields.map((field, index) => (
                            <div key={field.id} className="space-y-2">
                                <div className="flex flex-col md:flex-row gap-2">
                                    <FormField
                                        control={form.control}
                                        name={`taskInputData.${index}.inputData.data`}
                                        render={({ field }) => (
                                            <FormItem className="flex-1">
                                                <FormControl>
                                                    <Textarea
                                                        placeholder="Input"
                                                        className="resize-none min-h-[40px]"
                                                        {...field}
                                                    />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />

                                    <FormField
                                        control={form.control}
                                        name={`taskInputData.${index}.answer`}
                                        render={({ field }) => (
                                            <FormItem className="flex-1">
                                                <FormControl>
                                                    <Textarea
                                                        placeholder="Expected output"
                                                        className="resize-none min-h-[40px]"
                                                        {...field}
                                                    />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />

                                    <Button
                                        type="button"
                                        className="w-full md:w-fit btn-animation btn-red"
                                        onClick={() => remove(index)}
                                    >
                                        Remove
                                    </Button>
                                </div>
                            </div>
                        ))}
                    </div>
                    <div className="flex flex-col md:flex-row justify-between gap-4">
                        <Button
                            type="button"
                            onClick={() =>
                                append({
                                    idTaskProgramming: null,
                                    idInputDataTask: null,
                                    inputData: { idInputData: null, data: "" },
                                    answer: "",
                                })
                            }
                            className="btn-animation"
                        >
                            Add Test Case
                        </Button>
                        <Button
                            type="button"
                            onClick={reloadInputDatas}
                            className="btn-animation"
                        >
                            Reload input data
                        </Button>

                        <Button
                            type="button"
                            onClick={openInputDataModal}
                            className="btn-animation"
                        >
                            Add by Ready
                        </Button>
                    </div>
                </div>



                <Button type="submit" disabled={isLoading} className="w-full md:w-fit btn-animation">
                    {isLoading ? "Saving..." : submitLabel}
                </Button>
            </form>
        </Form>
    );
}

export default TaskForm;
