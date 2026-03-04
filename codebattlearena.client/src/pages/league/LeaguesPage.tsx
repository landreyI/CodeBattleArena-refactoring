import EmptyState from "@/components/common/EmptyState";
import ErrorMessage from "@/components/common/ErrorMessage";
import LoadingScreen from "@/components/common/LoadingScreen";
import { LeagueCard } from "@/components/cards/LeagueCard";
import { usePlayersLeagues } from "@/hooks/league/usePlayersLeagues";
import { useDeleteLeague } from "@/hooks/league/useDeleteLeague";
import { League } from "@/models/dbModels";
import InlineNotification from "@/components/common/InlineNotification";

export function LeaguesPage() {
    const { playersLeagues, setPlayersLeagues, loadPlayersLeagues, loading: playersLeaguesLoad, error: playersLeaguesError } = usePlayersLeagues();
    const { deleteLeague, error: deleteError } = useDeleteLeague();

    const handleDeletLeague = async (idLeague: number) => {
        const success = await deleteLeague(idLeague);
        if (success) 
            setPlayersLeagues((prevLeagues) => prevLeagues.filter((leaguePlayers) => leaguePlayers.league?.idLeague !== idLeague));
    }

    const handleUpdateLeague = async (leagueUpdate: League) => {
        setPlayersLeagues((prevLeagues) =>
            prevLeagues.map((leaguePlayers) =>
                leaguePlayers.league?.idLeague === leagueUpdate.idLeague
                    ? { ...leaguePlayers, league: leagueUpdate }
                    : leaguePlayers
            )
        );
    };

    if (playersLeaguesLoad) return <LoadingScreen />
    if (playersLeaguesError) return <ErrorMessage error={playersLeaguesError} />;
    if (!playersLeagues) return <EmptyState message="Leagues not found" />;

    return (
        <>
            {deleteError && <InlineNotification message={deleteError.message} className="bg-red" />}

            <div className="w-full lg:w-[70%] mx-auto">
                {playersLeagues.map((playersLeague, index) => (
                    <LeagueCard
                        key={index}
                        className="mb-5"
                        league={playersLeague.league}
                        players={playersLeague.players}
                        isEdit={true}
                        handleDeletLeague={() => playersLeague.league?.idLeague && handleDeletLeague(playersLeague.league?.idLeague)}
                        handleUpdateLeague={handleUpdateLeague}
                    />
                ))}
            </div>
        </>

    );
}

export default LeaguesPage;