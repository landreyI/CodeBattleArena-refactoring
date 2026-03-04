import { Item, TypeItem } from "@/models/dbModels";

interface Props {
    item?: Item;
    className?: string;
    children?: React.ReactNode;
}

export function BackgroundItem({ item, className, children }: Props) {
    const isDefaultBackground =
        item?.type !== TypeItem.Background ||
        !item?.cssClass?.trim() &&
        !item?.imageUrl?.trim();

    return (
        <div
            className={`${isDefaultBackground ? "background-default" : item?.cssClass ?? ""} ${className}`}
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

export default BackgroundItem;
