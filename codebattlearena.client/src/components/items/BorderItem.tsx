import { Item, TypeItem } from "@/models/dbModels";

interface Props {
    item?: Item;
    className?: string;
    children?: React.ReactNode;
}
export function BorderItem({ item, className, children }: Props) {
    const isDefaultBorder =
        item?.type !== TypeItem.Border ||
        !item?.cssClass?.trim() &&
        !item?.imageUrl?.trim();

    return (
        <div
            className={`${isDefaultBorder ? "" : item?.cssClass ?? ""} ${className}`}
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

export default BorderItem;