import { Card, CardContent } from "@/components/ui/card";
import { Session } from "@/models/dbModels";
import { Badge } from "../ui/badge";
import { Users, Code2, Calendar, Gamepad2 } from "lucide-react";
import { getIsStartGameColor, getStateColor } from "@/untils/helpers";

interface SessionCardProps {
    session: Session;
    className?: string;
}

export function SessionCard({ session, className }: SessionCardProps){
    return (
        <Card className={`${className}`}>
            <CardContent className="p-6">
                <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-2 mb-4">
                    <div className="text-xl font-bold font-mono">{session.name || "Unnamed"}</div>
                    <div className="">
                        <Badge className={getIsStartGameColor(session.status)}>
                            <Gamepad2 size={14} className="mr-1" />
                            {session.status.toString()}
                        </Badge>
                    </div>

                    <div className="flex items-center gap-2">
                        <Badge className={getStateColor(session.state)}>{session.state}</Badge>
                    </div>
                </div>

                <hr className="mb-4" />

                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 text-sm">
                    <div className="flex flex-col gap-2">
                        <div className="flex items-center gap-2">
                            <Code2 size={16} />
                            <span className="font-mono">Lang:</span>
                            <span>{session.programmingLang?.name || "Unknown"}</span>
                        </div>
                        <div className="flex items-center gap-2">
                            <Calendar size={16} />
                            <span className="font-mono">Created:</span>
                            <span>
                                {new Date(session.dateCreating).toLocaleDateString()}
                            </span>
                        </div>
                    </div>
                </div>
            </CardContent>
        </Card>
    );
};

export default SessionCard;