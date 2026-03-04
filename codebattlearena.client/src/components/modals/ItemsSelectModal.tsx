import {
    Dialog,
    DialogContent,
    DialogFooter,
} from "@/components/ui/dialog";

import { Item } from "@/models/dbModels";
import ItemFilter from "../filters/ItemFilter";
import ItemMiniCard from "../cards/ItemMiniCard";
import { Button } from "../ui/button";
import { ItemFilters } from "@/models/filters";
import { useMemo, useState } from "react";


interface Props {
    open: boolean;
    items: Item[]
    onSaveSelect?: (idItem: number | null) => void;
    onClose: () => void;
}
export function ItemsSelectModal({ open, items, onSaveSelect, onClose }: Props) {
    const [filters, setFilters] = useState<ItemFilters>({});
    const filteredItems = useMemo(() => {
        return items
            .filter(item =>
                (!filters.name || item.name.toLowerCase().includes(filters.name.toLowerCase())) &&
                (!filters.type || item.type === filters.type) &&
                (!filters.coin || (item.priceCoin ?? 0) <= filters.coin)
            )
            .sort((a, b) =>
                filters.isCoinDescending
                    ? (b.priceCoin ?? 0) - (a.priceCoin ?? 0)
                    : (a.priceCoin ?? 0) - (b.priceCoin ?? 0)
            );
    }, [filters, items]);

    return (
        <Dialog open={open} onOpenChange={onClose}>
            <DialogContent className="w-full sm:max-w-[99vw] md:max-w-[95vw] max-h-[95vh] overflow-y-auto">
                <ItemFilter
                    filter={filters}
                    onChange={setFilters}
                    handleSearch={() => { }}
                />

                <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-4 max-h-[55vh] overflow-y-auto pr-2">
                    {filteredItems?.map((item) => (
                        <div key={item.idItem} className="flex flex-col items-center gap-2 border-3 rounded-xl p-2">
                            <div className="relative h-full">
                                <ItemMiniCard item={item} />
                            </div>
                            <Button
                                type="button"
                                size="sm"
                                variant="outline"
                                onClick={() => onSaveSelect?.(item.idItem)}
                            >
                                Select
                            </Button>
                        </div>
                    ))}
                </div>

                <DialogFooter>
                    <Button variant="outline" onClick={onClose} className="btn-animation">
                        Cancel
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
}

export default ItemsSelectModal;