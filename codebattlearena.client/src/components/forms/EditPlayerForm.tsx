import { z } from "zod"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"

import {
    Form,
    FormControl,
    FormField,
    FormItem,
    FormLabel,
    FormMessage,
} from "@/components/ui/form"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import { Textarea } from "@/components/ui/textarea"
import { useEffect } from "react"
import { Player } from "@/models/dbModels"
import { useEditPlayer } from "@/hooks/player/useEditPlayer"

interface Props {
    player: Player,
    onUpdate: (updatedPlayer: Player) => void;
    onClose?: () => void;
}

export const formSchema = z.object({
    username: z
        .string()
        .min(2, { message: "Username must be at least 2 characters long." })
        .max(30, { message: "Username must not exceed 30 characters." }),
    additionalInformation: z
        .string()
        .max(500, { message: "Additional information should not exceed 500 characters.." })
        .nullable(),
});

export function EditPlayerForm({ player, onUpdate, onClose }: Props) {
    const { editPlayer, isLoading, error } = useEditPlayer();

    const form = useForm<z.infer<typeof formSchema>>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            username: player.username,
            additionalInformation: player.additionalInformation || "",
        },
    })

    useEffect(() => {
        form.reset({
            username: player.username,
            additionalInformation: player.additionalInformation || "",
        });
    }, [player, form]);

    const onSubmit = async (values: z.infer<typeof formSchema>) => {
        try {
            const updatedPlayer = await editPlayer(player, values);
            onUpdate(updatedPlayer);
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
                    <p className="text-red-400">{form.formState.errors.root.message}</p>
                )}
                <FormField
                    control={form.control}
                    name="username"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>UserName</FormLabel>
                            <FormControl>
                                <Input type="text" placeholder="your nickname" {...field} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name="additionalInformation"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Additional information</FormLabel>
                            <FormControl>
                                <Textarea placeholder="your information" {...field} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <Button type="submit" disabled={isLoading} className="inline-block mt-4 px-6 py-2 btn-animation">
                    {isLoading ? "Saving..." : "Save"}
                </Button>
            </form>
        </Form>
    );
}

export default EditPlayerForm;