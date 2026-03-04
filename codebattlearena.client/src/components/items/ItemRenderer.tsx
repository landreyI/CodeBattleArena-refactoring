import { Item, TypeItem } from "@/models/dbModels";
import BackgroundItem from "../items/BackgroundItem";
import { typeItemClassMap } from "@/untils/helpers";
import AvatarItem from "./AvatarItem";
import BadgeItem from "./BadgeItem";
import BorderItem from "./BorderItem";
import TitleItem from "./TitleItem";

interface Props {
    item?: Item;
    isAutoSize?: boolean;
    className?: string;
    children?: React.ReactNode;
}

export function ItemRenderer({ item, isAutoSize = true, className, children }: Props) {
    if (!item) {
        return <>{children}</>;
    }

    const defaultSizeClass = isAutoSize ? typeItemClassMap[item.type] ?? "w-auto h-auto" : "";

    switch (item.type) {
        case TypeItem.Background:
            return <BackgroundItem item={item} className={`${defaultSizeClass} ${className}`}>{children}</BackgroundItem>;
        case TypeItem.Avatar:
            return <AvatarItem item={item} className={`${defaultSizeClass} ${className}`}>{children}</AvatarItem>;
        case TypeItem.Badge:
            return <BadgeItem item={item} className={`${defaultSizeClass} ${className}`}>{children}</BadgeItem>;
        case TypeItem.Border:
            return <BorderItem item={item} className={`${defaultSizeClass} ${className}`}>{children}</BorderItem>;
        case TypeItem.Title:
            return <TitleItem item={item} className={`${defaultSizeClass} ${className}`}>{children}</TitleItem>;
        default:
            return (
                <div className={`w-full ${className}`}>
                    {children}
                </div>
            );
    }
}

export default ItemRenderer;

