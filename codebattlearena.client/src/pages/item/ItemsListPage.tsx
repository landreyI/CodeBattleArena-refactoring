import { TypeItem } from "@/models/dbModels";
import EmptyState from "@/components/common/EmptyState";
import ErrorMessage from "@/components/common/ErrorMessage";
import LoadingScreen from "@/components/common/LoadingScreen";
import { useLocation } from "react-router-dom";
import { ItemFilters } from "@/models/filters";
import { useState } from "react";
import { parseEnumParam } from "@/untils/helpers";
import ItemFilter from "@/components/filters/ItemFilter";
import { useItems } from "@/hooks/item/useItems";
import ItemsList from "@/components/lists/ItemsList";
import { usePlayerItems } from "@/hooks/item/usePlayerItems";
import InlineNotification from "@/components/common/InlineNotification";
import { useAuth } from "@/contexts/AuthContext";
import { PlayerProvider } from "@/contexts/PlayerContext";
import { usePlayer } from "@/hooks/player/usePlayer";

export function ItemsListPage() {
    const location = useLocation();
    const queryParams = new URLSearchParams(location.search);

    const name = queryParams.get('name') ?? "";
    const type = queryParams.get('typeItem') ? parseEnumParam(queryParams.get('typeItem'), TypeItem, TypeItem.Background) : undefined;
    const coin = queryParams.get('coin') ? Number(queryParams.get('coin')) : undefined;
    const isCoinDescending = Boolean(queryParams.get('isCoinDescending') ?? "");

    const filterReceived: ItemFilters = {
        type,
        name,
        coin,
        isCoinDescending
    };

    const [filter, setFilter] = useState<ItemFilters>(filterReceived);

    const { user } = useAuth();
    const { player } = usePlayer(user?.id);
    const { items, setItems, loading: itemsLoad, error: itemsError, reloadItems } = useItems(filter);
    const { playerItems, error: playerItemsError } = usePlayerItems(user?.id);

    const handleChangeFilter = (filter: ItemFilters) => {
        setFilter(filter);
    }

    if (itemsLoad) return <LoadingScreen />
    if (itemsError) return <ErrorMessage error={itemsError} />;

    const error = playerItemsError;

    return (
        <>
            {error && <InlineNotification message={error.message} className="bg-red" />}

            <ItemFilter filter={filter} onChange={handleChangeFilter} handleSearch={reloadItems}></ItemFilter>

            {(!items || items?.length === 0) && (<EmptyState message="Items not found" />)}

            <PlayerProvider player={player}>
                <ItemsList
                    items={items}
                    playerItems={playerItems}
                    cardWrapperClassName="hover:scale-[1.02] transition min-w-[250px]"
                />
            </PlayerProvider>
        </>
    )
}

export default ItemsListPage;