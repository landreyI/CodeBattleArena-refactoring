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
import { Item, TypeItem } from "@/models/dbModels";
import { useNavigate } from "react-router-dom";
import { useUpdateItem } from "@/hooks/item/useUpdateItem";
import { useCreateItem } from "@/hooks/item/useCreateItem";
import { useState } from "react";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";

// Определяем схему валидации формы
export const formSchema = z
    .object({
        name: z.string().min(3, { message: "League name must be at least 3 characters." }),
        type: z.nativeEnum(TypeItem, {
            errorMap: () => ({ message: "Invalid type item" }),
        }),
        priceCoin: z.preprocess(
            (val) => (val === "" || val == null ? undefined : Number(val)),
            z.number().min(0, { message: "Minimum 0" }).optional()
        ),
        cssClass: z.string().optional().refine(val => !val || val.length >= 3, {
            message: "Css class must be at least 3 characters.",
        }),
        imageUrl: z.string().optional().refine(val => !val || val.length >= 3, {
            message: "Image URL must be at least 3 characters.",
        }),
        description: z.string().optional().refine(val => !val || val.length >= 5, {
            message: "Description must be at least 5 characters.",
        }),
    });

interface Props {
    item?: Item,
    onClose?: () => void;
    onUpdate?: (updatedItem: Item) => void;
    submitLabel?: string;
}

export function ItemForm({ item, onClose, onUpdate, submitLabel }: Props) {
    const { createItem, loading: createIsLoad, error: createError } = useCreateItem();
    const { updateItem, loading: updateIsLoad, error: updateError } = useUpdateItem();

    const [typeItem, setTypeItem] = useState<TypeItem>(item?.type || TypeItem.Background);
    // Подменяем в зависимости от контекста (создание или редактирование)
    const isEditing = !!item;
    
    const isLoading = isEditing ? updateIsLoad : createIsLoad;
    const error = isEditing ? updateError : createError;


    const navigate = useNavigate();

    const form = useForm<z.infer<typeof formSchema>>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            name: item?.name || "",
            type: item?.type || TypeItem.Background,
            priceCoin: item?.priceCoin || undefined,
            cssClass: item?.cssClass || "",
            imageUrl: item?.imageUrl || "",
            description: item?.description || "",
        },
    });

    function buildLeagueData(values: z.infer<typeof formSchema>, item?: Item): Item {
        return {
            idItem: item?.idItem ?? null,

            name: values.name,
            type: values.type,
            priceCoin: values.priceCoin ?? null,
            cssClass: values.cssClass ?? null,
            imageUrl: values.imageUrl ?? null,
            description: values.description ?? null,
        };
    }

    const onSubmit = async (values: z.infer<typeof formSchema>) => {
        try {
            let result = null;

            const itemData = buildLeagueData(values, item);
            result = await (item ? updateItem(itemData) : createItem(itemData));

            if (onUpdate && item)
                onUpdate(itemData);

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
                            <FormLabel>Item Name</FormLabel>
                            <FormControl>
                                <Input placeholder="Enter item name" {...field} />
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
                            <FormLabel>Type Item</FormLabel>
                            <FormControl>
                                <Select
                                    onValueChange={(val) => {
                                        field.onChange(val);
                                        setTypeItem(val as TypeItem); // сохраняем в useState
                                    }}
                                    value={field.value}>
                                    <SelectTrigger>
                                        <SelectValue placeholder="Select state" />
                                    </SelectTrigger>
                                    <SelectContent>
                                        {Object.values(TypeItem).map((typeValue) => (
                                            <SelectItem key={typeValue} value={typeValue}>
                                                {typeValue}
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
                    name="priceCoin"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel className="flex flex-col items-start">Price coin
                                <span className="text-muted-foreground">(If you want the item to be unpurchasable, leave this field blank.)</span>
                            </FormLabel>
                            <FormControl>
                                <Input
                                    type="number"
                                    placeholder="Enter price"
                                    {...field}
                                />
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
                            <FormLabel>Description (Optional)</FormLabel>
                            <FormControl>
                                <Input placeholder="Enter description" {...field} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                <FormField
                    control={form.control}
                    name="cssClass"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Css class (Optional)</FormLabel>
                            <FormControl>
                                <Input placeholder="Enter class" {...field} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                <FormField
                    control={form.control}
                    name="imageUrl"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Url Background (Optional)</FormLabel>
                            <FormControl>
                                <Input placeholder="Enter url" {...field} />
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

export default ItemForm;
