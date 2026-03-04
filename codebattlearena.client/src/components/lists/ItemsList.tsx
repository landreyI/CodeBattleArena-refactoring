import { Item } from "@/models/dbModels";
import { ItemMiniCard } from "../cards/ItemMiniCard";
import { motion } from "framer-motion";
import IconButton from "../buttons/IconButton";
import { Trash2 } from "lucide-react";
interface Props {
    items: Item[],
    playerItems?: Item[],
    cardWrapperClassName?: string;
    onDelete?: (idItem: number) => void;
}
export function ItemsList({ items, playerItems, cardWrapperClassName, onDelete }: Props) {

    return (
        <motion.div
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: 10 }}
            transition={{ duration: 0.3 }}
            className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-5 mt-5 [grid-auto-rows:1fr]"
        >
            {items.map((item) => {
                const isOwned = playerItems?.some(p => p.idItem === item.idItem);
                return (
                    <div key={item.idItem} className="relative h-full">
                        <ItemMiniCard
                            item={item}
                            className={`h-full flex flex-col ${cardWrapperClassName ?? ''}`}
                            isOwned={isOwned}
                        >
                            {onDelete && item.idItem != null && (
                                <IconButton icon={<Trash2 className="w-4 h-4" />} onClick={() => onDelete(item.idItem! ?? "")} />
                            )}
                        </ItemMiniCard>
                    </div>
                );
            })}
        </motion.div>
    );
}

export default ItemsList;