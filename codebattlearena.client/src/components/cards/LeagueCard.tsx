import {
    Card,
    CardHeader,
    CardTitle,
    CardContent,
    CardFooter,
    CardAction,
} from "../ui/card";
import { ShieldCheck, ChevronDown, ChevronUp } from "lucide-react";
import { League, Player } from "@/models/dbModels";
import { PlayersList } from "../lists/PlayersList";
import clsx from "clsx";
import { useState } from "react";
import { Button } from "../ui/button";
import SettingMenu from "../menu/SettingMenu";
import EditModal from "../modals/EditModal";
import LeagueForm from "../forms/LeagueForm";

interface Props {
    league?: League;
    players?: Player[];
    className?: string;
    isEdit?: boolean
    handleDeletLeague: () => void;
    handleUpdateLeague: (leagueUpdate: League) => void;
}

export function LeagueCard({ league, players, className, isEdit, handleDeletLeague, handleUpdateLeague }: Props) {
    const [showPlayers, setShowPlayers] = useState(true);
    const name = league?.name ?? "unknown";
    const [showEditLeague, setShowEditLeague] = useState(false);

    return (
        <>
            <div
                className={clsx(
                    "flex flex-col md:flex-row gap-6 items-start w-full",
                    className
                )}
            >
                {/* Левая карточка лиги */}
                <Card className={clsx("w-full md:w-[60%] pb-25 rounded-0", "clip-custom-shape", "league-" + name.toLowerCase())}>
                    <CardHeader className="px-6 pt-2 pb-2">
                        <div className="flex flex-col items-center text-center gap-1">
                            <div className="flex items-center gap-2 text-xl">
                                <ShieldCheck size={20} className="text-primary" />
                                <CardTitle className="font-mono">{name} League</CardTitle>
                                {isEdit && (
                                    <SettingMenu
                                        setShowEdit={setShowEditLeague}
                                        handleDelet={handleDeletLeague}
                                    />
                                )}
                            </div>
                            <span className="text-m text-muted-foreground">
                                Victories: {league?.minWins} - {league?.maxWins ?? "∞"}
                            </span>
                        </div>
                    </CardHeader>

                    <CardContent className="flex justify-between items-center px-6 pb-2">
                        <span className="text-m text-muted-foreground">{players?.length ?? 0} Players</span>
                        <CardAction>
                            <Button
                                onClick={() => setShowPlayers((prev) => !prev)}
                                size="icon"
                                variant="ghost"
                                className="rounded-full hover:bg-muted transition"
                            >
                                {showPlayers ? <ChevronUp size={18} /> : <ChevronDown size={18} />}
                            </Button>
                        </CardAction>
                    </CardContent>

                    <CardFooter className="flex justify-center px-4 pb-4 hover:scale-[1.1] transition">
                        <img
                            src={`${league?.photoUrl}`}
                            alt={`${name} emblem`}
                            className="object-contain select-none pointer-events-none"
                        />
                    </CardFooter>
                </Card>


                {/* Правая часть — список игроков */}
                <div
                    className={clsx(
                        "transition-all duration-500 ease-in-out overflow-hidden w-full p-2" + " league-" + name.toLowerCase(),
                        showPlayers ? "max-h-[1000px] opacity-100" : "max-h-0 opacity-0"
                    )}
                >
                    {players && players.length > 0 ? (
                        <PlayersList
                            players={players}
                            cardWrapperClassName="w-full hover:scale-[1.02] transition"
                            isNumbered={true}
                        />
                    ) : (
                        <div className="text-muted-foreground text-center text-sm">
                            No players in this league
                        </div>
                    )}
                </div>
            </div>
            {league && (
                <EditModal open={showEditLeague} title="Edit League" onClose={() => setShowEditLeague(false)}>
                    <LeagueForm league={league} onClose={() => setShowEditLeague(false)} onUpdate={handleUpdateLeague} submitLabel="Save"></LeagueForm>
                </EditModal>
            )}
        </>
    );
}

export default LeagueCard;
