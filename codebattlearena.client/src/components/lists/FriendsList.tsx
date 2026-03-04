import { Friend } from "@/models/dbModels";
import { motion } from "framer-motion";
import PlayerMiniCard from "../cards/PlayerMiniCard";
import { useAuth } from "@/contexts/AuthContext";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Trash2 } from "lucide-react";
import IconButton from "../buttons/IconButton";

interface Props {
    friends: Friend[];
    renderItemAddon?: (friend: Friend) => React.ReactNode;
    className?: string;
    cardWrapperClassName?: string;
    onDelete?: (friendId?: number) => void;
    onApprove?: (friendId?: number) => void;
}

export function FriendsList({ friends, renderItemAddon, className, cardWrapperClassName, onDelete, onApprove }: Props) {
    const { user } = useAuth();

    return (
        <motion.div
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: 10 }}
            transition={{ duration: 0.3 }}
            className={`grid gap-3 ${className ?? ""}`}
        >
            {friends.map((friend) => {
                const isCurrentUserAddressee = friend.addressee?.id === user?.id;
                const isCurrentUserRequester = friend.requester?.id === user?.id;
                const targetPlayer = isCurrentUserAddressee ? friend.requester : friend.addressee;

                return (
                    <div
                        key={friend.idFriend}
                        className={`flex flex-col md:flex-row gap-2 items-center bg-card border relative rounded-xl overflow-hidden ${cardWrapperClassName ?? ""}`}
                    >
                        <PlayerMiniCard
                            player={targetPlayer ?? undefined}
                            className="border-none"
                        >
                            {!friend.isFriendship && (
                                <>
                                    {isCurrentUserAddressee && (
                                        <IconButton icon={"Approve"} variant="default" className="btn-animation w-fit px-2" onClick={() => onApprove?.(friend?.idFriend ?? undefined)} />
                                    )}

                                    {isCurrentUserRequester && (
                                        <Badge>Expectation</Badge>
                                    )}
                                </>
                            )}
                            {onDelete && (
                                <IconButton icon={<Trash2 className="w-4 h-4" />} onClick={() => onDelete(friend?.idFriend ?? undefined)} />
                            )}
                        </PlayerMiniCard>
                        {renderItemAddon?.(friend)}
                    </div>
                );
            })}
        </motion.div>
    );
}

export default FriendsList;
