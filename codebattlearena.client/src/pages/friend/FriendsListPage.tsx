import EmptyState from "@/components/common/EmptyState";
import ErrorMessage from "@/components/common/ErrorMessage";
import LoadingScreen from "@/components/common/LoadingScreen";
import InlineNotification from "@/components/common/InlineNotification";
import { useFriends } from "@/hooks/friend/useFriends";
import { useDeleteFriend } from "@/hooks/friend/useDeleteFriend";
import { FriendsList } from "@/components/lists/FriendsList";
import { useApproveFriendship } from "@/hooks/friend/useApproveFriendship";
import { usePlayerEventsHub } from "@/hooks/hubs/player/usePlayerEventsHub";
import { Friend, Player } from "@/models/dbModels";
import { useAuth } from "@/contexts/AuthContext";

export function FriendsListPage() {
    const { friends, setFriends, loading: friendsLoad, error: friendsError, reloadFriends } = useFriends();
    const { deleteFriend, error: deleteError, loading: deleteLoad } = useDeleteFriend();
    const { approveFriendship, loading: approveLoad, error: approveError } = useApproveFriendship();
    const { user } = useAuth();

    const handleDeletFriend = async (friendId?: number) => {
        const success = await deleteFriend(friendId);
        if (success)
            setFriends((prevFriends) => prevFriends.filter((friend) => friend.idFriend !== friendId));
    };

    const handleApproveFriend = async (friendId?: number) => {
        const success = await approveFriendship(friendId);
        if (success && friendId !== undefined) {
            setFriends((prevFriends) =>
                prevFriends.map((friend) =>
                    friend.idFriend === friendId
                        ? { ...friend, isFriendship: true }
                        : friend
                )
            );
        }
    };

    usePlayerEventsHub({
        onFriendRequest: (sender: Player) => {
            reloadFriends();
        }
    })

    if (friendsLoad) return <LoadingScreen />
    if (friendsError) return <ErrorMessage error={friendsError} />;

    const error = deleteError || approveError;

    return (
        <>
            {error && <InlineNotification message={error.message} className="bg-red" />}

            {!friends || friends.length === 0 && (<EmptyState message="Friends not found" />)}

            <div className="mb-5"></div>

            <FriendsList
                friends={friends}
                cardWrapperClassName="hover:scale-[1.02] transition"
                onDelete={handleDeletFriend}
                onApprove={handleApproveFriend}
            />
        </>
    )
}

export default FriendsListPage;