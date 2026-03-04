import { Link, useParams } from "react-router-dom";

import { usePlayer } from "@/hooks/player/usePlayer";
import { useState } from "react";
import { Player, TypeItem } from "@/models/dbModels";
import PlayerCard from "@/components/cards/PlayerCard";
import SessionList from "@/components/lists/SessionsList";
import { Button } from "@/components/ui/button";
import { ChevronDown, ChevronRight, Trophy } from "lucide-react";
import LoadingScreen from "@/components/common/LoadingScreen";
import ErrorMessage from "@/components/common/ErrorMessage";
import EmptyState from "@/components/common/EmptyState";
import { usePlayerSessions } from "@/hooks/playerSession/usePlayerSessions";
import SettingMenu from "@/components/menu/SettingMenu";
import { Badge } from "@/components/ui/badge";
import BackgroundItem from "@/components/items/BackgroundItem";
import { ItemProvider } from "@/contexts/ItemContext";
import ItemRenderer from "@/components/items/ItemRenderer";
import { usePlayerItems } from "@/hooks/item/usePlayerItems";
import { Carousel, CarouselContent, CarouselItem, CarouselNext, CarouselPrevious } from "@/components/ui/carousel";
import { LevelDisplay } from "@/components/common/LevelDisplay";
import { isAdminRole } from "@/untils/businessRules";
import { useAuth } from "@/contexts/AuthContext";
import { MenuAction } from "@/components/menu/GenericDropdownMenu";
import RoleSelectModal from "@/components/modals/RoleSelectModal";
import { useSelectRoles } from "@/hooks/player/useSelectRoles";
import InlineNotification from "@/components/common/InlineNotification";
import { useAddFriend } from "@/hooks/friend/useAddFriend";
import EditPlayerForm from "@/components/forms/EditPlayerForm";
import EditModal from "@/components/modals/EditModal";
import { Skeleton } from "@/components/ui/skeleton";

export function PlayerPage() {
    const { playerId } = useParams<{ playerId: string }>();
    const { player, setPlayer, isEdit, loading: playerLoad, error: playerError } = usePlayer(playerId);
    const { sessions, error: sessionsError } = usePlayerSessions(playerId, isEdit);
    const { playerItems: bages } = usePlayerItems(playerId, TypeItem.Badge);
    const { selectRoles, loading: selectLoad, error: selectError } = useSelectRoles();
    const { addFriend, loading: addFriendLoad, error: addFriendError } = useAddFriend();
    const { user } = useAuth();
    const [notification, setNotification] = useState<string | null>(null);

    const [showEditPlayer, setShowEditPlayer] = useState(false);
    const [showSelectRoles, setShowSelectRoles] = useState(false);

    const [showSessions, setShowSessions] = useState(true);

    const handleUpdatePlayer = (updatedPlayer: Player) => {
        setPlayer(updatedPlayer);
    };

    const handleAddFriend = async () => {
        setNotification(null);
        const success = await addFriend(playerId);
        if (success) {
            setNotification("Friend request sent");
        }
    }

    const handleOnSelecRoles = async (roles: string[]) => {
        setNotification(null);
        const success = await selectRoles(playerId, roles);
        if (success) {
            setNotification("Roles updated successfully");
            setPlayer(prev => {
                if (!prev) return prev;
                return {
                    ...prev,
                    roles: roles
                };
            });
        }
    }

    if (playerLoad || selectLoad) {
        return (
            <div className="w-full min-h-[95vh] p-3">
                <div className="w-full md:w-[65vw] mx-auto space-y-3">
                    {/* Верхний блок */}
                    <div className="grid grid-cols-1 md:grid-cols-[1fr_15vw] gap-3">
                        <div className="relative">
                            <Skeleton className="w-full h-[200px] rounded-xl" />
                        </div>
                        <div className="flex flex-col gap-2">
                            <div className="bg-primary rounded-xl p-3 space-y-2">
                                <Skeleton className="w-3/4 h-6 bg-primary-pressed/30" />
                                <Skeleton className="w-1/2 h-4 bg-primary-pressed/30" />
                            </div>
                            <div className="bg-muted-card w-full rounded-xl p-3">
                                <Skeleton className="w-1/3 h-10" />
                            </div>
                            <Skeleton className="w-full h-12 rounded-xl " />
                        </div>
                    </div>

                    {/* Нижний блок */}
                    <div className="grid grid-cols-1 md:grid-cols-[1fr_15vw] gap-3">
                        <div className="border-card border-5 rounded-xl shadow-sm w-full">
                            <div className="bg-card px-3 py-1 rounded-t-md flex justify-between items-center h-10">
                                <Skeleton className="w-24 h-8" />
                                <Skeleton className="w-8 h-8 rounded-md" />
                            </div>
                            <div className="flex flex-col md:flex-row p-3 gap-3 h-[250px]">
                                {[...Array(3)].map((_, i) => (
                                    <Skeleton key={i} className="w-full h-full  rounded-xl" />
                                ))}
                            </div>
                        </div>

                        <div className="bg-muted-card w-full flex flex-col rounded-xl p-3 gap-3">
                            <div className="flex gap-2">
                                {[...Array(3)].map((_, i) => (
                                    <Skeleton key={i} className="w-full h-15 rounded-xl" />
                                ))}
                            </div>
                            <Skeleton className="w-2/3 h-4" />
                            <Skeleton className="w-full h-8" />
                            <Skeleton className="w-full h-8" />
                            <Skeleton className="w-full h-8" />
                            <Skeleton className="w-1/2 h-4" />
                        </div>
                    </div>
                </div>
            </div>
        );
    }

    if (playerError) return <ErrorMessage error={playerError} />;
    if (!player) return <EmptyState message="Player not found" />;

    const error = sessionsError || selectError || addFriendError;

    const isEditRole = isAdminRole(user?.roles ?? []);

    const actions: MenuAction[] = [
        { label: "Select role", onClick: () => setShowSelectRoles(true), shortcut: "⌘R" },
    ];

    return (
        <>
            {error && <InlineNotification message={error.message} className="bg-red" />}

            {notification && (
                <InlineNotification message={notification} className="bg-blue" />
            )}

            <BackgroundItem item={player.activeBackground ?? undefined} className="w-full min-h-[95vh] rounded-2xl p-3">
                <div className="w-full md:w-[65vw] mx-auto p-3 rounded-xl space-y-3">
                    {/* Верхний блок: карточка + меню + статистика */}
                    <div className="grid grid-cols-1 md:grid-cols-[1fr_15vw] gap-3">
                        <div className="relative">
                            {isEdit && (
                                <div className="absolute top-0 right-0 z-10 bg-muted-card p-1 rounded-xl">
                                    <SettingMenu setShowEdit={setShowEditPlayer} actionsProp={actions} />
                                </div>
                            )}
                            <PlayerCard className="shadow-none border-none bg-transparent p-0" player={player} />
                        </div>

                        <div className="flex flex-col gap-2">
                            <div className="flex flex-col items-center bg-primary rounded-xl gap-1 p-3 justify-between">
                                <LevelDisplay experience={player?.experience ?? 0} />

                                <Badge className="flex items-center gap-2 bg-primary-pressed rounded-xl text-white">
                                    <Trophy size={16} />
                                    <p className="text-sm font-mono font-semibold">VICTORIES</p>
                                    <p className="text-sm font-bold">{player.victories}</p>
                                </Badge>
                            </div>

                            <div className="flex flex-row items-center bg-muted-card w-full rounded-xl p-3 gap-3">
                                Badge
                                <ItemRenderer item={player.activeBadge ?? undefined} />
                            </div>
                            {playerId !== user?.id && (
                                <Button className="btn-animation rounded-xl!" onClick={handleAddFriend}>
                                    Add Friend
                                </Button>
                            )}
                        </div>
                    </div>


                    {/* Нижний блок: sessions + info */}
                    <div className="grid grid-cols-1 md:grid-cols-[1fr_15vw] gap-3">
                        <div className="border-card border-5 rounded-xl shadow-sm w-full">
                            <div className="flex items-center justify-between bg-card px-3 py-1 rounded-t-md">
                                <h2 className="text-xl text-primary">Sessions</h2>
                                <Button
                                    variant="ghost"
                                    size="icon"
                                    onClick={() => setShowSessions((prev) => !prev)}
                                >
                                    {showSessions ? <ChevronDown className="w-5 h-5" /> : <ChevronRight className="w-5 h-5" />}
                                </Button>
                            </div>

                            <ItemProvider item={player.activeBorder ?? null}>
                                <div className="max-h-[70vh] overflow-y-auto">
                                    <div className="p-3">
                                        {showSessions && (
                                            <SessionList
                                                sessions={sessions}
                                                cardWrapperClassName="hover:bg-border hover:scale-[1.02] transition border-2"
                                            />
                                        )}
                                    </div>
                                </div>
                            </ItemProvider>
                        </div>

                        <div className="bg-muted-card w-full flex flex-col rounded-xl p-3 gap-3">
                            <Carousel className="w-full max-w-full">
                                <CarouselContent className="-ml-2">
                                    {bages.map((bage, index) => (
                                        <CarouselItem key={index} className="pl-1 basis-1/2 md:basis-1/2 lg:basis-1/3">
                                            <ItemRenderer item={bage} />
                                        </CarouselItem>
                                    ))}
                                </CarouselContent>
                                <CarouselPrevious className="-left-7 rounded-xl size-7" />
                                <CarouselNext className="-right-7 rounded-xl size-7" />
                            </Carousel>
                            <p>Games {player.countGames ?? 0}</p>
                            <Link to={`/item/player-items/${playerId}`} className="nav-link">
                                Inventory
                            </Link>
                            <Link to={`/item/list-items`} className="nav-link">
                                Shop Items
                            </Link>
                            <Link to={`/task/my-tasks/${playerId}`} className="nav-link">
                                My Tasks
                            </Link>
                            <Link to={`/friend/list-friends`} className="nav-link">
                                Friends
                            </Link>
                            <p>Chats</p>
                        </div>
                    </div>
                </div>
            </BackgroundItem>

            {player && (
                <EditModal open={showEditPlayer} title="Edit Player" onClose={() => setShowEditPlayer(false)}>
                    <EditPlayerForm player={player} onClose={() => setShowEditPlayer(false)} onUpdate={handleUpdatePlayer}></EditPlayerForm>
                </EditModal>
            )}
            {player && (
                <RoleSelectModal
                    open={showSelectRoles}
                    onSelect={handleOnSelecRoles}
                    defaultSelected={player.roles ?? []}
                    onClose={() => setShowSelectRoles(false)}
                />
            )}
        </>
    );
};

export default PlayerPage;