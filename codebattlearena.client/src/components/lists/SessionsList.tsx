import { Session } from "@/models/dbModels";
import { SessionMiniCard } from "@/components/cards/SessionMiniCard";
import { motion } from "framer-motion";
interface Props {
    sessions: Session[],
    cardWrapperClassName?: string;
    className?: string;
}

export function SessionList({ sessions, cardWrapperClassName, className }: Props) {

    return (
        <motion.div
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: 10 }}
            transition={{ duration: 0.3 }}
            className={`grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-3 ${className}`}
        >
            {sessions.map((session) => (
                <SessionMiniCard
                    key={session.idSession}
                    session={session}
                    className={cardWrapperClassName}
                />
            ))}
        </motion.div>
    );
}
export default SessionList;