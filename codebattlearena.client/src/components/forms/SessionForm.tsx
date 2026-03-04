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
import { SessionState, Session } from "@/models/dbModels";
import { CreateSessionCommand, UpdateSessionCommand } from "@/models/dtoCommands";
import { useNavigate } from "react-router-dom";
import { useLangsProgramming } from "@/hooks/useLangsProgramming";
import LoadingScreen from "../common/LoadingScreen";
import ErrorMessage from "../common/ErrorMessage";
import EmptyState from "../common/EmptyState";
import { useState } from "react";
import InputPassword from "../common/InputPassword";
import { useCreateSession } from "@/hooks/session/useCreateSession";
import { useUpdateSession } from "@/hooks/session/useUpdateSession";
import { useActiveSession } from "@/contexts/ActiveSessionContext";
import { getStateColor } from "@/untils/helpers";

export const formSchema = z
    .object({
        name: z.string().min(2, "Session name must be at least 2 characters."),
        maxPeople: z.coerce.number().min(1, "Minimum 1 person").max(10, "Maximum 10 people"),
        idLangProgramming: z.string().min(1, "Select language"),
        state: z.nativeEnum(SessionState),
        // Используем coerce вместо preprocess для стабильного вывода типа number
        timePlay: z.coerce.number().min(5).max(60).optional(),
        // Пароль живет только в форме и командах, в Session его нет
        password: z.string().optional(),
    })
    .superRefine((data, ctx) => {
        if (data.state === SessionState.Private && (!data.password || data.password.trim() === "")) {
            ctx.addIssue({
                path: ["password"],
                code: z.ZodIssueCode.custom,
                message: "Password is required for private sessions",
            });
        }
    });

// Явный тип для значений формы
export type SessionFormValues = z.infer<typeof formSchema>;

interface Props {
    session?: Session;
    onClose?: () => void;
    onUpdate?: (updatedSession: Session) => void;
    submitLabel?: string;
}

export function SessionForm({ session, onClose, onUpdate, submitLabel = "Save" }: Props) {
    const { langsProgramming, loading, error: langsError } = useLangsProgramming();
    const { createSession, loading: createIsLoad, error: createError } = useCreateSession();
    const { updateSession, loading: updateIsLoad, error: updateError } = useUpdateSession();
    const { refreshSession } = useActiveSession();

    const isEditing = !!session;
    const isLoading = isEditing ? updateIsLoad : createIsLoad;
    const serverError = isEditing ? updateError : createError;

    const [state, setState] = useState<SessionState>(session?.state || SessionState.Public);
    const navigate = useNavigate();

    const form = useForm<SessionFormValues>({ // Используем созданный нами тип
        resolver: zodResolver(formSchema),
        defaultValues: {
            name: session?.name || "",
            maxPeople: session?.maxPeople || 1,
            // Важно: приводим к строке сразу здесь
            idLangProgramming: session?.programmingLangId?.toString() || "",
            state: session?.state || SessionState.Public,
            timePlay: session?.timePlay || 5,
            password: "",
        },
    });

    if (loading) return <LoadingScreen />;
    if (langsError) return <ErrorMessage error={langsError} />;
    if (!langsProgramming) return <EmptyState message="Langs programming not found" />;

    const onSubmit = async (values: SessionFormValues) => {
        try {
            if (isEditing && session) {
                const command: UpdateSessionCommand = {
                    id: session.id, // Из твоего интерфейса Session
                    name: values.name,
                    programmingLangId: values.idLangProgramming,
                    state: values.state,
                    maxPeople: values.maxPeople,
                    password: values.state === SessionState.Private ? values.password : null,
                    timePlay: values.timePlay,
                    taskId: session.taskId
                };

                await updateSession(session.id, command);
                // Обновляем локальный стейт, чтобы не делать лишний запрос
                if (onUpdate) onUpdate({ ...session, ...command } as Session);

            } else {
                const command: CreateSessionCommand = {
                    name: values.name,
                    programmingLangId: values.idLangProgramming,
                    state: values.state,
                    maxPeople: values.maxPeople,
                    password: values.state === SessionState.Private ? values.password : null,
                    timePlay: values.timePlay,
                    taskId: null
                };

                const newSessionId = await createSession(command);

                // Проверяем, что пришел валидный GUID
                if (newSessionId && typeof newSessionId === 'string') {
                    refreshSession();
                    navigate(`/session/info-session/${newSessionId}`);
                }
            }

            if (onClose) onClose();
        } catch (err) {
            console.error("Submit error:", err);
            // Используем ошибку с сервера, если она есть
            form.setError("root", { message: serverError?.message || "Failed to process request" });
        }
    };

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
                {/* ... (ошибка root) */}

                <FormField
                    control={form.control}
                    name="name"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Session Name</FormLabel>
                            <FormControl>
                                <Input placeholder="Enter session name" {...field} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                <div className="grid grid-cols-2 gap-4">
                    <FormField
                        control={form.control}
                        name="maxPeople"
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel>Max Participants</FormLabel>
                                <FormControl>
                                    <Input type="number" {...field} />
                                </FormControl>
                                <FormMessage />
                            </FormItem>
                        )}
                    />

                    <FormField
                        control={form.control}
                        name="timePlay"
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel>Duration (min)</FormLabel>
                                <FormControl>
                                    <Input type="number" placeholder="5-60" {...field} />
                                </FormControl>
                                <FormMessage />
                            </FormItem>
                        )}
                    />
                </div>

                <FormField
                    control={form.control}
                    name="idLangProgramming"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Programming Language</FormLabel>
                            {/* defaultValue критически важен для Shadcn Select */}
                            <Select onValueChange={field.onChange} defaultValue={field.value}>
                                <FormControl>
                                    <SelectTrigger>
                                        <SelectValue placeholder="Select language" />
                                    </SelectTrigger>
                                </FormControl>
                                <SelectContent>
                                    {langsProgramming.map((lang) => (
                                        <SelectItem key={lang.idLang} value={lang.idLang.toString()}>
                                            {lang.nameLang}
                                        </SelectItem>
                                    ))}
                                </SelectContent>
                            </Select>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                <FormField
                    control={form.control}
                    name="state"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Session State</FormLabel>
                            <Select
                                onValueChange={(val) => {
                                    field.onChange(val);
                                    setState(val as SessionState);
                                }}
                                defaultValue={field.value}
                            >
                                <FormControl>
                                    <SelectTrigger className={getStateColor(state)}>
                                        <SelectValue placeholder="Select state" />
                                    </SelectTrigger>
                                </FormControl>
                                <SelectContent>
                                    {Object.values(SessionState).map((s) => (
                                        <SelectItem key={s} value={s} className={getStateColor(s)}>
                                            {s}
                                        </SelectItem>
                                    ))}
                                </SelectContent>
                            </Select>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                {state === SessionState.Private && (
                    <FormField
                        control={form.control}
                        name="password"
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel>Password</FormLabel>
                                <FormControl>
                                    {/* Гарантируем, что value не null */}
                                    <InputPassword placeholder="Enter password" {...field} value={field.value || ""} />
                                </FormControl>
                                <FormMessage />
                            </FormItem>
                        )}
                    />
                )}

                <Button type="submit" disabled={isLoading} className="w-full btn-animation">
                    {isLoading ? "Saving..." : submitLabel}
                </Button>
            </form>
        </Form>
    );
}

export default SessionForm;