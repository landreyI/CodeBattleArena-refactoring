import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { Player } from "@/models/dbModels";
import { BookOpenText, Calendar, Mail, Trophy } from "lucide-react";
import clsx from "clsx";
import { useState } from "react";
import { getRoleColor } from "@/untils/helpers";
import AvatarPlayer from "../avatars/AvatarPlayer";
import ItemRenderer from "../items/ItemRenderer";
interface Props {
    player: Player;
    className?: string;
}
export function PlayerCard({ player, className }: Props) {
    const [showFullBio, setShowFullBio] = useState<boolean>();

    return (
        <Card className={`shadow-lg flex flex-col md:flex-row p-3 rounded-xl ${className}`}>
            <ItemRenderer
                item={player.activeAvatar ?? undefined}
                isAutoSize={false}
                className=""
            >
                <AvatarPlayer
                    photoUrl={player.photoUrl ?? undefined}
                    username={player.username}
                    className="w-40 h-40"
                    classNameImage="hover:scale-[1.3] transition"
                />
            </ItemRenderer>

            <div className="flex flex-col gap-4 justify-between w-full p-1">
                <div>
                    <div className="flex flex-col sm:flex-row items-center gap-2">
                        <CardTitle className="text-2xl font-semibold bg-muted-card w-fit p-1 rounded-xl">
                            {player.username}
                        </CardTitle>
                        <ItemRenderer
                            item={player.activeTitle ?? undefined}
                            isAutoSize={false}
                            className="text-md font-semibold w-fit p-1 rounded-xl"
                        />
                    </div>

                    <div className="flex flex-wrap gap-2 mt-2">
                        {player.roles?.map((role, index) => (
                            <Badge key={index} className={getRoleColor(role)}>
                                {role}
                            </Badge>
                        ))}
                    </div>
                </div>

                <div className="flex flex-col gap-1">
                    {player.additionalInformation && (
                        <div className="">
                            <div className="flex items-center gap-2 mb-1 bg-muted-card p-1 rounded-xl w-fit">
                                <BookOpenText size={16} />
                                <p className="text-sm font-mono">Bio:</p>
                            </div>
                            <p
                                onClick={() => setShowFullBio(prev => !prev)}
                                className={clsx(
                                    "text-m cursor-pointer transition-all duration-300 break-words whitespace-pre-wrap overflow-hidden bg-muted-card p-1 rounded-xl",
                                    showFullBio
                                        ? "line-clamp-none max-h-[1000px]"
                                        : "line-clamp-2 max-h-[3.6em]"
                                )}
                                title="Click to expand"
                            >
                                {player.additionalInformation}
                            </p>
                        </div>
                    )}

                    <div className="flex items-center gap-2 text-sm bg-muted-card p-1 rounded-xl w-fit ml-auto">
                        <Calendar size={16} />
                        <p className="font-mono">Joined:</p>
                        <p>{new Date(player.createdAt).toLocaleDateString()}</p>
                    </div>
                </div>
            </div>
        </Card>
    );
}

export default PlayerCard;