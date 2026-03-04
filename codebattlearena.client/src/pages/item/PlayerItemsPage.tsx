import EmptyState from "@/components/common/EmptyState";
import ErrorMessage from "@/components/common/ErrorMessage";
import LoadingScreen from "@/components/common/LoadingScreen";
import { useParams } from "react-router-dom";
import ItemsList from "@/components/lists/ItemsList";
import { usePlayerItems } from "@/hooks/item/usePlayerItems";
import { usePlayer } from "@/hooks/player/usePlayer";
import { PlayerProvider } from "@/contexts/PlayerContext";

export function PlayerItemsPage() {
    const { playerId } = useParams<{ playerId: string }>();
    const { player } = usePlayer(playerId);
    const { playerItems, loading: playerItemsLoad, error: playerItemsError } = usePlayerItems(playerId);

    if (playerItemsLoad) return <LoadingScreen />
    if (playerItemsError) return <ErrorMessage error={playerItemsError} />;

    return (
        <>
            {!playerItems || playerItems.length === 0 && (<EmptyState message="Items not found" />)}

            <PlayerProvider player={player}>
                <ItemsList
                    items={playerItems}
                    cardWrapperClassName="hover:scale-[1.02] transition"
                />
            </PlayerProvider>
        </>
    )
}

export default PlayerItemsPage;