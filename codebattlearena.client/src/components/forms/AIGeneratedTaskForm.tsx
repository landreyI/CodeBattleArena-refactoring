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
import { Button } from "@/components/ui/button";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Difficulty, TaskProgramming} from "@/models/dbModels";
import { useLangsProgramming } from "@/hooks/useLangsProgramming";
import LoadingScreen from "../common/LoadingScreen";
import ErrorMessage from "../common/ErrorMessage";
import EmptyState from "../common/EmptyState";
import { Textarea } from "../ui/textarea";
import { getDifficultyColor } from "@/untils/helpers";
import { GenerateAITaskProgramming } from "@/models/models";
import { useAIGenerateTask } from "@/hooks/task/useAIGenerateTask";

// Определяем схему валидации формы
export const formSchema = z
    .object({
        idLangProgramming: z.number().min(0, { message: "Select language" }),
        difficulty: z.nativeEnum(Difficulty, {
            errorMap: () => ({ message: "Invalid difficulty" }),
        }),
        promt: z.string(),
    })

interface Props {
    idTaskProgramming?: number;
    onClose?: () => void;
    onUpdate?: (updatedTask: TaskProgramming) => void;
    updateLoading?: (update: boolean) => void;
    submitLabel?: string;
}

export function AIGeneratedTaskForm({ idTaskProgramming, onClose, onUpdate, updateLoading, submitLabel }: Props) {
    const { langsProgramming, loading: langsLoad, error: langsError } = useLangsProgramming();
    const { generateTask, loading: createIsLoad, error: createError } = useAIGenerateTask();

    const isLoading = createIsLoad;
    const error = createError;

    updateLoading?.(isLoading);

    const form = useForm<z.infer<typeof formSchema>>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            idLangProgramming: -1,
            difficulty: Difficulty.Easy,
            promt: "",
        },
    });


    if (langsLoad) return <LoadingScreen />
    if (langsError) return <ErrorMessage error={langsError} />;
    if (!langsProgramming) return <EmptyState message="Langs programming not found" />;

    function buildTaskData(values: z.infer<typeof formSchema>): GenerateAITaskProgramming {
        return {
            langProgramming: langsProgramming?.find(lang => lang.idLang === values.idLangProgramming) ?? null,
            langProgrammingId: values.idLangProgramming,
            difficulty: values.difficulty,
            promt: values.promt,
        };
    }

    const onSubmit = async (values: z.infer<typeof formSchema>) => {
        try {
            let result = null;

            const taskData = buildTaskData(values);
            taskData.idTaskProgramming = idTaskProgramming;
            result = await generateTask(taskData);

            if (onUpdate && result)
                onUpdate(result);

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
                    name="promt"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Promt</FormLabel>
                            <FormControl>
                                <Textarea placeholder="Enter promt" {...field} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                <Button type="submit" disabled={isLoading} className="w-full md:w-fit btn-animation">
                    {isLoading ? "Ganerating..." : submitLabel}
                </Button>
            </form>
        </Form>
    );
}

export default AIGeneratedTaskForm;
