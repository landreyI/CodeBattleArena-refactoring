import { Player } from "@/models/dbModels";
import PlayerMiniCard from "../cards/PlayerMiniCard";
import { motion } from "framer-motion";
import IconButton from "../buttons/IconButton";
import { Terminal, Trash2 } from "lucide-react";

interface Props {
    players: Player[],
    onDelete?: (playerId: string) => void;
    onPlayerSessionInfo?: (playerId: string) => void;
    renderItemAddon?: (player: Player) => React.ReactNode;
    cardWrapperClassName?: string;
    isTop?: boolean;
    isNumbered?: boolean;
}

export function PlayersList({
    players,
    onDelete,
    cardWrapperClassName,
    onPlayerSessionInfo,
    renderItemAddon,
    isTop = false,
    isNumbered = false
}: Props) {
    const sortedPlayers = isTop
        ? [...players].sort((a, b) => (b.victories ?? 0) - (a.victories ?? 0))
        : players;

    const getTopStyle = (index: number) => {
        if (!isTop) return "";

        switch (index) {
            case 0:
                return "border-3 border-yellow";
            case 1:
                return "border-3 border-gray";
            case 2:
                return "border-3 border-bronze";
            default:
                return "";
        }
    };

    return (
        <motion.div
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: 10 }}
            transition={{ duration: 0.3 }}
            className="grid gap-3"
        >
            {sortedPlayers.map((player, index) => (
                <div key={player.id} className={`relative overflow-hidden  ${cardWrapperClassName ?? ""}`}>
                    {isTop && index < 3 && (
                        <div className="absolute top-1 left-2 bg-primary bg-opacity-60 text-xs px-2 py-0.5 rounded-md z-10">
                            TOP {index + 1}
                        </div>
                    )}
                    <PlayerMiniCard
                        player={player}
                        number={isNumbered ? index + 1 : undefined}
                        className={`${getTopStyle(index)}`}
                    >
                        {onPlayerSessionInfo && (
                            <IconButton icon={<Terminal className="w-4 h-4" />} onClick={() => onPlayerSessionInfo(player?.id ?? "")} />
                        )}
                        {onDelete && (
                            <IconButton icon={<Trash2 className="w-4 h-4" />} onClick={() => onDelete(player?.id ?? "")} />
                        )}
                    </PlayerMiniCard>
                    {renderItemAddon?.(player)}
                </div>
            ))}
        </motion.div>
    );
}
