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
import { League } from "@/models/dbModels";
import { useNavigate } from "react-router-dom";
import { useCreateLeague } from "@/hooks/league/useCreateLeague";
import { useUpdateLeague } from "@/hooks/league/useUpdateLeague";


// Определяем схему валидации формы
export const formSchema = z
    .object({
        name: z.string().min(3, { message: "League name must be at least 3 characters." }),
        photoUrl: z.string().optional(),
        minWins: z.coerce.number().min(0, { message: "Minimum 0 Wins" }),
        maxWins: z.coerce.number().min(0, { message: "Minimum 0 Wins" }).optional(),
    });

interface Props {
    league?: League,
    onClose?: () => void;
    onUpdate?: (updatedItem: League) => void;
    submitLabel?: string;
}

export function LeagueForm({ league, onClose, onUpdate, submitLabel }: Props) {
    const { createLeague, loading: createIsLoad, error: createError } = useCreateLeague();
    const { updateLeague, loading: updateIsLoad, error: updateError } = useUpdateLeague();

    // Подменяем в зависимости от контекста (создание или редактирование)
    const isEditing = !!league;

    const isLoading = isEditing ? updateIsLoad : createIsLoad;
    const error = isEditing ? updateError : createError;


    const navigate = useNavigate();

    const form = useForm<z.infer<typeof formSchema>>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            name: league?.name || "",
            photoUrl: league?.photoUrl,
            minWins: league?.minWins || 0,
            maxWins: league?.maxWins || undefined,
        },
    });

    function buildLeagueData(values: z.infer<typeof formSchema>, league?: League): League {
        return {
            idLeague: league?.idLeague ?? null,

            name: values.name,
            photoUrl: values.photoUrl,
            minWins: values.minWins,
            maxWins: values.maxWins ?? null,
        };
    }

    const onSubmit = async (values: z.infer<typeof formSchema>) => {
        try {
            let result = null;

            const leagueData = buildLeagueData(values, league);
            result = await (league ? updateLeague(leagueData) : createLeague(leagueData));

            if (onUpdate && league)
                onUpdate(leagueData);

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
                            <FormLabel>League Name</FormLabel>
                            <FormControl>
                                <Input placeholder="Enter league name" {...field} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                <FormField
                    control={form.control}
                    name="photoUrl"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>League photo url</FormLabel>
                            <FormControl>
                                <Input placeholder="Enter url" {...field} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                <FormField
                    control={form.control}
                    name="minWins"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Minimum number of wins</FormLabel>
                            <FormControl>
                                <Input type="number" placeholder="Enter number" {...field} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                <FormField
                    control={form.control}
                    name="maxWins"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Maximum number of wins</FormLabel>
                            <FormControl>
                                <Input type="number" placeholder="Enter number" {...field} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                <Button type="submit" disabled={isLoading} className="w-full md:w-fit btn-animation">
                    {isLoading ? "Saving..." : submitLabel}
                </Button>
            </form>
        </Form>
    );
}

export default LeagueForm;
