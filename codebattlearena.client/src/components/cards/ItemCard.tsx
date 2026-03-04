import { Card, CardContent } from "@/components/ui/card";
import { Item } from "@/models/dbModels";
import { Coins, FileText, Tag } from "lucide-react";
import ItemRenderer from "../items/ItemRenderer";

interface Props {
    item: Item;
    className?: string;
}

export function ItemCard({ item, className }: Props) {
    return (
        <Card className={`border shadow-md w-full ${className}`}>
            <CardContent className="p-6 space-y-6">
                <div className="text-2xl font-bold font-mono flex items-center gap-2">
                    <span>{item.name || "Unnamed Item"}</span>
                    {item.priceCoin != null && (
                        <span className="flex items-center gap-1 text-yellow">
                            <Coins className="w-5 h-5" />
                            {item.priceCoin} coin
                        </span>
                    )}
                </div>

                <ItemRenderer item={item} className="rounded-xl" />

                <div className="grid gap-4 text-sm">
                    <div className="flex items-start gap-3">
                        <Tag size={18} className="text-muted-foreground mt-0.5 shrink-0" />
                        <div>
                            <div className="text-xs font-medium text-muted-foreground tracking-wide">
                                Type
                            </div>
                            <div className="text-base font-medium">
                                {item.type || "No type"}
                            </div>
                        </div>
                    </div>

                    <div className="flex items-start gap-3">
                        <FileText size={18} className="text-muted-foreground mt-0.5 shrink-0" />
                        <div>
                            <div className="text-xs font-medium text-muted-foreground tracking-wide">
                                Description
                            </div>
                            <div className="whitespace-pre-wrap text-sm">
                                {item.description || "No description"}
                            </div>
                        </div>
                    </div>
                </div>
            </CardContent>
        </Card>
    );
}

export default ItemCard;
