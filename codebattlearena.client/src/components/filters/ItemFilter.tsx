import { useEffect, useState } from "react";
import { ItemFilters } from "@/models/filters";
import { TypeItem } from "@/models/dbModels";
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select";
import { Button } from "../ui/button";
import { Card } from "../ui/card";
import { Input } from "../ui/input";
import { Label } from "../ui/label";
import { Switch } from "../ui/switch";

interface Props {
    filter: ItemFilters;
    onChange: (filter: ItemFilters) => void;
    handleSearch: () => void;
}

const getTypeItems = () => {
    return Object.keys(TypeItem) as Array<keyof typeof TypeItem>;
};

export function ItemFilter({ filter, onChange, handleSearch }: Props) {
    const [selectedType, setSelectedType] = useState<string>(filter.type || "all");
    const [name, setName] = useState<string>(filter.name || "");
    const [coin, setCoin] = useState<number | undefined>(filter.coin || undefined)
    const [isCoinDescending, setIsCoinDescending] = useState<boolean>(filter.isCoinDescending)

    useEffect(() => {
        setSelectedType(filter.type || "all");
        setName(filter.name || "");
        setCoin(filter.coin || undefined);
        setIsCoinDescending(filter.isCoinDescending);
    }, [filter]);

    const handleTypeItemChange = (value: string) => {
        setSelectedType(value);
    };

    const handleNameChange = (value: string) => {
        setName(value);
    };

    const handleCoinChange = (value: string) => {
        setCoin(Number(value));
    };

    const handleIsCoinDescending = (value: boolean) => {
        setIsCoinDescending(value);
    };

    const handleSearchClick = () => {
        const updatedFilter: ItemFilters = {
            ...filter,
            type: selectedType === "all" ? undefined : (selectedType as TypeItem),
            name: name.trim() !== "" ? name.trim() : undefined,
            coin: coin,
            isCoinDescending: isCoinDescending
        };
        onChange(updatedFilter);
        handleSearch();
    };

    return (
        <Card className="p-2 sm:p-4">
            <div className="flex flex-col gap-4 md:flex-row md:items-center md:gap-6 flex-wrap">
                <div className="flex flex-col gap-4 md:flex-row md:gap-6">
                    <div className="flex flex-col gap-1">
                        <Label className="text-sm font-medium">Type items</Label>
                        <div className="flex flex-row gap-1">
                            <Select
                                value={selectedType}
                                onValueChange={handleTypeItemChange}
                            >
                                <SelectTrigger className="w-full md:w-[180px] border-gray-300 focus:ring-2 focus:ring-blue-500">
                                    <SelectValue placeholder="All types" />
                                </SelectTrigger>
                                <SelectContent>
                                    <SelectItem value="all">All types</SelectItem>
                                    {Object.values(TypeItem).map((type) => (
                                        <SelectItem key={type} value={type}>
                                            {type}
                                        </SelectItem>
                                    ))}
                                </SelectContent>
                            </Select>
                        </div>
                    </div>
                    <div className="flex flex-col gap-1">
                        <Label className="text-sm font-medium">Name</Label>
                        <Input
                            placeholder="Enter item name"
                            value={name}
                            onChange={(e) => handleNameChange(e.target.value)}
                            className="w-full"
                        />
                    </div>
                    <div className="flex flex-col gap-1">
                        <Label className="text-sm font-medium">Price coin</Label>
                        <Input
                            placeholder="Enter item price"
                            value={coin}
                            onChange={(e) => handleCoinChange(e.target.value)}
                            className="w-full"
                        />
                    </div>
                    <div className="flex flex-col gap-1">
                        <Label className="text-sm font-medium">Ascending or Descending</Label>
                        <Switch
                            checked={isCoinDescending}
                            onCheckedChange={handleIsCoinDescending}
                        />
                    </div>
                </div>
                <Button
                    className="w-full md:w-auto btn-animation px-4 py-2"
                    onClick={handleSearchClick}
                >
                    Search
                </Button>
            </div>
        </Card>
    );
}


export default ItemFilter;