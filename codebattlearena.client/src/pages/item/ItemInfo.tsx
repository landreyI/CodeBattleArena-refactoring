import { useNavigate, useParams } from "react-router-dom";
import { useItem } from "@/hooks/item/useItem";
import { useDeleteItem } from "@/hooks/item/useDeleteItem";
import { useAuth } from "@/contexts/AuthContext";
import { useEffect, useState } from "react";
import { Friend, Item, PlayerItem } from "@/models/dbModels";
import EmptyState from "@/components/common/EmptyState";
import ErrorMessage from "@/components/common/ErrorMessage";
import LoadingScreen from "@/components/common/LoadingScreen";
import InlineNotification from "@/components/common/InlineNotification";
import { isEditRole } from "@/untils/businessRules";
import ItemCard from "@/components/cards/ItemCard";
import SettingMenu from "@/components/menu/SettingMenu";
import { usePlayerItems } from "@/hooks/item/usePlayerItems";
import { CircleCheck} from "lucide-react";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { useBuyItem } from "@/hooks/item/useBuyItem";
import { useChangeActiveItem } from "@/hooks/player/useChangeActiveItem";
import { useFriendshipFriends } from "@/hooks/friend/useFriendshipFriends";
import FriendsSelectModal from "@/components/modals/FriendsSelectModal";
import EditModal from "@/components/modals/EditModal";
import ItemForm from "@/components/forms/ItemForm";

export function ItemInfo() {
    const { itemId } = useParams<{ itemId: string }>();
    const { item, setItem, loading: itemLoad, error: itemError, reloadItem } = useItem(Number(itemId));
    const { deleteItem, error: itemDeleteError } = useDeleteItem();
    const { buyItem, error: playerItemCreateError } = useBuyItem();
    const { changeActiveItem, error: changeError } = useChangeActiveItem();
    const { user, setUser } = useAuth();
    const { playerItems } = usePlayerItems(user?.id);
    const { friendships, loading: friendshipsLoad, error: friendshipsError, reloadFriendships } = useFriendshipFriends();
    const navigate = useNavigate();

    const [notification, setNotification] = useState<string | null>(null);
    const [showEditItem, setShowEditItem] = useState(false);
    const [showSelectedFriends, setShowSelectedFriends] = useState(false);
    const [isOwned, setIsOwned] = useState(false);

    useEffect(() => {
        setIsOwned(playerItems?.some(p => p.idItem === item?.idItem) ?? false);
    }, [playerItems, item]);


    const handleUpdateItem = (updatedItem: Item) => {
        setItem(updatedItem);
    };

    const handleDeletItem = async () => {
        const isSuccess = await deleteItem(Number(itemId));
        if (isSuccess)
            navigate(`/item/list-items`);
    };

    const handleAddInventory = async () => {
        setNotification(null);
        const playerItem: PlayerItem = {
            idPlayer: user?.id ?? "",
            player: null,
            idItem: item?.idItem ?? 0,
            item: null
        };
        const isSuccess = await buyItem(playerItem);
        if (isSuccess) {
            setNotification("The item has been added to your inventory.");
            setUser(prev =>
                prev
                    ? {
                        ...prev,
                        coin: (prev.coin ?? 0) - (item?.priceCoin ?? 0),
                    }
                    : prev
            );
            setIsOwned(true);
        }
    }

    const handleMakeActive = async () => {
        setNotification(null);
        const isSuccess = await changeActiveItem(user?.id, Number(itemId));
        if (isSuccess) setNotification("Successfully change");
    }

    const handlePressSelectedFriends = () => {
        if (!friendships || friendships.length === 0)
            reloadFriendships();
        setShowSelectedFriends(true);
    }

    const handleSelectedFriend = async (selectedFriend?: Friend) => {
        setShowSelectedFriends(false);
        setNotification(null);
        const playerItem: PlayerItem = {
            idPlayer: selectedFriend?.addresseeId !== user?.id ? selectedFriend?.addresseeId ?? "" : selectedFriend?.requesterId ?? "",
            player: null,
            idItem: item?.idItem ?? 0,
            item: null
        };
        const isSuccess = await buyItem(playerItem);
        if (isSuccess) {
            setNotification("This item has been gifted successfully.");
            setIsOwned(true);
        }
    };

    if (itemLoad || friendshipsLoad) return <LoadingScreen />
    if (itemError) return <ErrorMessage error={itemError} />;
    if (!item) return <EmptyState message="Item not found" />;

    const error = itemDeleteError || playerItemCreateError || changeError || friendshipsError;
    return (
        <>
            {error && <InlineNotification message={error.message} className="bg-red" />}

            {notification && (
                <InlineNotification message={notification} className="bg-blue" />
            )}

            <div className="glow-box py-8 px-4">
                <div className="md:w-[60vw] sm:w-[100vw] mx-auto space-y-4">
                    <div className="flex flex-col md:flex-row items-center justify-between gap-4 w-full">
                        {/* Левая зона: бейдж + кнопка */}
                        <div className="flex flex-col md:flex-row items-center gap-4">
                            {isOwned && (
                                <Badge className="inline-flex items-center gap-2 bg-green px-3 py-1.5 rounded-md text-sm font-semibold">
                                    <CircleCheck size={16} />
                                    In Inventory
                                </Badge>
                            )}

                            {user && (
                                <>
                                    <Button
                                        onClick={isOwned ? handleMakeActive : handleAddInventory}
                                        className="btn-animation w-auto"
                                    >
                                        {isOwned ? 'Set as Active' : 'Get item'}
                                    </Button>
                                    {item.priceCoin && (
                                        <Button
                                            className="btn-animation w-auto"
                                            onClick={handlePressSelectedFriends}
                                        >
                                            Gift for friend
                                        </Button>
                                    )}
                                </>
                            )}
                        </div>

                        {/* Правая зона: меню */}
                        {user && isEditRole(user.roles) && (
                            <SettingMenu
                                setShowEdit={setShowEditItem}
                                handleDelet={handleDeletItem}
                            />
                        )}
                    </div>

                    <ItemCard item={item} />
                </div>
            </div>

            {item && (
                <EditModal open={showEditItem} title="Edit Item" onClose={() => setShowEditItem(false)}>
                    <ItemForm item={item} onClose={() => setShowEditItem(false)} onUpdate={handleUpdateItem} submitLabel="Save"></ItemForm>
                </EditModal>
            )}
            {friendships && (
                <FriendsSelectModal open={showSelectedFriends} friends={friendships} onClose={() => setShowSelectedFriends(false)} onSaveSelect={handleSelectedFriend} />
            )}
        </>
    )
}

export default ItemInfo;