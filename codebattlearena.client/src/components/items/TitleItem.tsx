import { Item, TypeItem } from "@/models/dbModels";

interface Props {
    item?: Item;
    className?: string;
    children?: React.ReactNode;
}
export function TitleItem({ item, className, children }: Props) {
    const isDefaultTitle =
        item?.type !== TypeItem.Title ||
        !item?.cssClass?.trim() &&
        !item?.imageUrl?.trim();

    return (
        <div
            className={`text-center px-2 ${isDefaultTitle ? "" : item?.cssClass ?? ""} ${className ?? ""}`}
            style={{
                backgroundImage: item?.imageUrl ? `url(${item.imageUrl})` : undefined,
                backgroundSize: "cover",
                backgroundPosition: "center",
            }}
        >
            {item?.name}
            {children}
        </div>
    );
}

export default TitleItem;