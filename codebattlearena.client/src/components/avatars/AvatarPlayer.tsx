import { Avatar, AvatarFallback, AvatarImage } from "../ui/avatar";
interface Props {
    photoUrl?: string;
    username?: string;
    classNameImage?: string;
    className?: string;
}
export function AvatarPlayer({ photoUrl, username, classNameImage, className }: Props) {
    return (
        <>
            <Avatar className={`rounded-xl text-primary text-2xl ${className}`}>
                <AvatarImage
                    src={photoUrl || undefined}
                    alt={username}
                    className={`${classNameImage}`}
                />
                <AvatarFallback>
                    {username?.charAt(0).toUpperCase() ?? "?"}
                </AvatarFallback>
            </Avatar>
        </>
    )
}

export default AvatarPlayer;