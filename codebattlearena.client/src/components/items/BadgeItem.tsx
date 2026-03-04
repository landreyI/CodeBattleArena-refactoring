import { Item, TypeItem } from "@/models/dbModels";

interface Props {
    item?: Item;
    className?: string;
    children?: React.ReactNode;
}
export function BadgeItem({ item, className, children }: Props) {
    const isDefaultBadge =
        item?.type !== TypeItem.Badge ||
        !item?.cssClass?.trim() &&
        !item?.imageUrl?.trim();

    return (
        <div
            className={`${isDefaultBadge ? "" : item?.cssClass ?? ""} ${className}`}
            style={{
                backgroundImage: item?.imageUrl ? `url(${item.imageUrl})` : undefined,
                backgroundSize: "cover",
                backgroundPosition: "center",
            }}
        >
            {children}
        </div>
    );
}

export default BadgeItem;