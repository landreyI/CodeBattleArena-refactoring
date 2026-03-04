import { Item } from "@/models/dbModels";
import { isActiveItem } from "@/untils/helpers";
import { CircleCheck, Coins, Sparkles } from "lucide-react";
import { Link } from "react-router-dom";
import ItemRenderer from "../items/ItemRenderer";
import { Badge } from "../ui/badge";
import { usePlayer } from "@/contexts/PlayerContext";

interface Props {
    item: Item;
    isOwned?: boolean;
    className?: string;
    children?: React.ReactNode;
}

export function ItemMiniCard({ item, isOwned, className, children }: Props) {
    const player = usePlayer();
    
    return (
        <Link to={`/item/info-item/${item.idItem}`} title="View Item">
            <div className={`${className}`}>
                {isOwned && (
                    <Badge className="z-1 absolute bg-green top-1 left-1 flex items-center gap-1">
                        <CircleCheck size={14} />
                    </Badge>
                )}

                {item.priceCoin != null && (
                    <Badge className="z-1 absolute top-1 left-1/2 -translate-x-1/2 bg-yellow flex items-center gap-1">
                        <Coins className="w-5 h-5" />
                        {item.priceCoin}
                    </Badge>
                )}

                {isActiveItem(item.idItem ?? undefined, player ?? undefined) && (
                    <Badge className="z-1 absolute right-1 top-1 bg-purple flex items-center gap-1">
                        <Sparkles size={14} />
                    </Badge>
                )}

                <ItemRenderer item={item} className={`rounded-xl mx-auto`} />

                {children}

                <div className="mt-auto bg-primary text-center text-black font-semibold rounded-xl w-auto px-2">
                    {item.name}
                </div>
            </div>
        </Link>
    );
};

export default ItemMiniCard;