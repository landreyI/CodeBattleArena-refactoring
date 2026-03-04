import { Item, TypeItem } from "@/models/dbModels";
import { typeItemClassMap } from "@/untils/helpers";

interface Props {
    item?: Item;
    className?: string;
    children?: React.ReactNode;
}
export function AvatarItem({ item, className, children }: Props) {
    const isDefaultAvatar =
        item?.type !== TypeItem.Avatar ||
        (!item?.cssClass?.trim() && !item?.imageUrl?.trim());

    return (
        <div className="relative inline-flex items-center justify-center">
            {children}

            {!isDefaultAvatar && (
                <div
                    className={`
                        absolute inset-0 pointer-events-none
                        bg-contain bg-no-repeat bg-center
                        ${item?.cssClass ?? ""}
                        ${className ?? ""}
                    `}
                    style={{
                        backgroundImage: item?.imageUrl
                            ? `url(${item.imageUrl})`
                            : undefined,
                    }}
                />
            )}
        </div>
    );
}


export default AvatarItem;