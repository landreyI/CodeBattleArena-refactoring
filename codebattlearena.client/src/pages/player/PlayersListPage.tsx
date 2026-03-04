import { usePlayersList } from "@/hooks/player/usePlayersList";
import EmptyState from "@/components/common/EmptyState";
import ErrorMessage from "@/components/common/ErrorMessage";
import LoadingScreen from "@/components/common/LoadingScreen";
import { PlayersList } from "@/components/lists/PlayersList";
import { useLocation } from "react-router-dom";
import { Role } from "@/models/dbModels";
import { parseEnumParam } from "@/untils/helpers";
import { PlayerFilters } from "@/models/filters";
import { useState } from "react";
import PlayerFilter from "@/components/filters/PlayerFilter";

export function PlayersListPage() {
    const location = useLocation();
    const queryParams = new URLSearchParams(location.search);

    const role = queryParams.get('role') ? parseEnumParam(queryParams.get('role'), Role, Role.User) : undefined;
    const userName = queryParams.get('userName') ?? "";

    const filterReceived: PlayerFilters = {
        role,
        userName
    };

    const [filter, setFilter] = useState<PlayerFilters>(filterReceived);

    const { players, setPlayers, loadPlayers, loading: playersLoad, error: playersError } = usePlayersList(filter);

    const handleChangeFilter = (filter: PlayerFilters) => {
        setFilter(filter);
    }

    if (playersLoad) return <LoadingScreen />
    if (playersError) return <ErrorMessage error={playersError} />;
    if (!players) return <EmptyState message="Tasks not found" />;

    return (
        <div className="w-full lg:w-[60%] mx-auto">

            {!players || players.length === 0 && (<EmptyState message="Players not found" />)}

            <PlayerFilter filter={filter} onChange={handleChangeFilter} handleSearch={loadPlayers}></PlayerFilter>

            <div className="mt-3"></div>

            <PlayersList
                players={players}
                cardWrapperClassName="w-full hover:scale-[1.02] transition"
                isTop={true}
                isNumbered={true}
            />
        </div>
    );
}

export default PlayersListPage;