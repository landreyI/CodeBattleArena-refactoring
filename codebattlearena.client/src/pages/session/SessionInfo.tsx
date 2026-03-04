import { useNavigate, useParams } from "react-router-dom";

import { useSession } from "@/hooks/session/useSession";
import { useState } from "react";
import { Difficulty, Friendship, Session, SessionState, GameStatus } from "@/models/dbModels";
import { SessionCard } from "@/components/cards/SessionCard";
import LoadingScreen from "@/components/common/LoadingScreen";
import ErrorMessage from "@/components/common/ErrorMessage";
import EmptyState from "@/components/common/EmptyState";
import { useSessionPlayers } from "@/hooks/session/useSessionPlayers";
import SettingMenu from "@/components/menu/SettingMenu";
import TaskProgrammingMiniCard from "@/components/cards/TaskProgrammingMiniCard";
import InlineNotification from "@/components/common/InlineNotification";
import { useDeleteSession } from "@/hooks/session/useDeleteSession";
import { Button } from "@/components/ui/button";
import { TaskProgrammingFilters } from "@/models/filters";
import { useSessionJoin } from "@/hooks/session/useSessionJoin";
import { useActiveSession } from "@/contexts/ActiveSessionContext";
import InputPassword from "@/components/common/InputPassword";
import { Users } from "lucide-react";
import { useSessionEventsHub } from "@/hooks/hubs/session/useSessionEventsHub";
import { useStartGame } from "@/hooks/session/useStartGame";
import { useAuth } from "@/contexts/AuthContext";
import { useFinishGame } from "@/hooks/session/useFinishGame";
import { useBestResult } from "@/hooks/session/useBestResult";
import CodeVerificationResult from "@/components/cards/CodeVerificationResult";
import { PlayersList } from "@/components/lists/PlayersList";
import { useCountCompletedTask } from "@/hooks/session/useCountCompletedTask";
import { useKickOutSession } from "@/hooks/session/useKickOutSession";
import { useFriendshipFriends } from "@/hooks/friend/useFriendshipFriends";
import FriendsSelectModal from "@/components/modals/FriendsSelectModal";
import { useInviteSession } from "@/hooks/session/useInviteSession";
import EditModal from "@/components/modals/EditModal";
import SessionForm from "@/components/forms/SessionForm";
import { formatDuration } from "@/untils/helpers";

export function SessionInfo() {
    const { sessionId } = useParams<{ sessionId: string }>();
    const { session, setSession, isEdit, loading: sessionLoad, error: sessionError, reloadSession } = useSession(sessionId);
    const { players, setPlayers, loading: playersLoad, reloadPlayers } = useSessionPlayers(sessionId, true);
    const { deleteSession, error: deleteError } = useDeleteSession();
    const [notification, setNotification] = useState<string | null>(null);
    const navigate = useNavigate();
    const [showEditSession, setShowEditSession] = useState(false);
    const { isCompleted, loading: joinLoad, error: joinError, joinSession } = useSessionJoin();
    const { error: startError, startGame } = useStartGame();
    const { error: finishError, finishGame } = useFinishGame();
    const { playerSession: bestResult, reloadBestResult } = useBestResult(sessionId);
    const { activeSession, setActiveSession, leaveSession } = useActiveSession();
    const { kickOutSession, error: kickError } = useKickOutSession();
    const { count, setCount, error: countCompletedError, reloadCountCompleted } = useCountCompletedTask(sessionId);
    const [password, setPassword] = useState<string>("");
    const { user } = useAuth();

    const { friendships, loading: friendshipsLoad, error: friendshipsError, reloadFriendships } = useFriendshipFriends();
    const { inviteSession, error: inviteError } = useInviteSession();
    const [showSelectedFriends, setShowSelectedFriends] = useState(false);

    useSessionEventsHub(sessionId, {
        onDelete: () => navigate("/home"),
        onUpdate: (sessionUpdate) => {
            if (session?.state !== sessionUpdate.state)
                reloadPlayers();

            reloadSession();
        },
        onJoin: () => reloadPlayers(),
        onLeave: () => reloadPlayers(),
        onKickOut: () => reloadPlayers(),
        onFinishGame: () => reloadBestResult(),
        onUpdateCountCompleted: (completedCount) => {
            setCount(prev => {
                if (!prev) return prev;
                return {
                    ...prev,
                    completed: completedCount
                };
            });
        }, 
    });

    const handleUpdateSession = (updatedSession: Session) => {
        setSession(updatedSession);
    };

    const handleDeletPlayer = async (playerId: string) => {
        const success = await kickOutSession(playerId, sessionId);
    };

    const handlePlayerSessionInfo = (playerId?: string) => {
        if (!playerId || !session?.id || session.status === GameStatus.Waiting) return;

        const query = new URLSearchParams({
            playerId: playerId,
            sessionId: session.id,
        });

        navigate(`/session/player-code?${ query.toString() }`);
    };

    const handleDeletSession = async () => {
        const success = await deleteSession(sessionId);
        await leaveSession();
    }

    const handleJoinSession = async () => {
        if (session && session.id) {
            const success = await joinSession(session.id, password);

            if (success) {
                setActiveSession(session);
            }
        }
    };

    const goToTaskList = () => {
        const filter: TaskProgrammingFilters = {
            idLang: session?.programmingLangId ?? undefined,
            difficulty: Difficulty.Easy,
        };

        const query = new URLSearchParams(filter as any).toString();

        navigate(`/task/list-task?${ query }`);
    };

    const handlePressSelectedFriends = () => {
        if (!friendships || friendships.length === 0)
            reloadFriendships();
        setShowSelectedFriends(true);
    }

    const handleSelectedFriend = async (selectedFriend?: Friendship[]) => {
        setShowSelectedFriends(false);
        setNotification(null);

        if (!selectedFriend || selectedFriend.length === 0 || !session || session.id) return;

        const friendIds = selectedFriend.map((friend) => {
            return friend.receiverId === user?.id
                ? friend.senderId
                : friend.receiverId;
        });

        const success = await inviteSession(session.id, friendIds);

        if (success) {
            setNotification("Invitations sent");
        }
    };

    if (sessionLoad) return <LoadingScreen />
    if (sessionError) return <ErrorMessage error={sessionError} />;
    if (!session) return <EmptyState message="Session not found" />;

    const isStarted = session.status == GameStatus.InProgress;
    const isFinished = session.status == GameStatus.Finished;
    const isPrivate = session?.state === SessionState.Private;
    const isJoined = activeSession?.id === session?.id;

    const error = deleteError || joinError || startError || finishError || kickError || countCompletedError || friendshipsError || inviteError;

    return (
        <>
            {error && <InlineNotification message={error.message} className="bg-red" />}

            {notification && (
                <InlineNotification message={notification} className="bg-blue" />
            )}

            <div className="glow-box">
                <div className="md:w-[40vw] sm:w-[100vw] mx-auto">
                    {/* Заголовок страницы */}
                    <div className="flex items-center justify-between mb-6">
                        <h1 className="text-4xl font-bold text-primary font-mono">
                            Session - details
                        </h1>
                        {isEdit && (
                            <SettingMenu
                                setShowEdit={setShowEditSession}
                                handleDelet={handleDeletSession}
                            />
                        )}
                    </div>

                    {bestResult && (
                        <div className="bg-card border p-4 rounded-2xl shadow-sm mb-4">
                            <h3 className="text-lg font-semibold text-primary mb-2">
                                🏆 Best Result: <span className="text-foreground">{bestResult?.player?.name}</span>
                            </h3>
                            <h2 className="text-lg font-semibold text-primary mb-2">
                                Time spent writing code:{" "}
                                {bestResult?.finishTask && session.dateStartGame
                                    ? formatDuration(new Date(bestResult.finishTask), new Date(session.dateStartGame))
                                    : "Not finished"}
                            </h2>

                            <div className="max-w-md">
                                <CodeVerificationResult
                                    executionResult={{
                                        time: bestResult.time ?? undefined,
                                        memory: bestResult.memory ?? undefined,
                                        compileOutput: undefined
                                    }}
                                    className="bg-primary text-black"
                                />
                            </div>
                        </div>
                    )}

                    {/* Карточка сессии */}
                    <SessionCard session={session}></SessionCard>

                    <div className="my-3 text-2xl font-bold text-primary">Task:</div>

                    {session.programmingTask && (
                        <TaskProgrammingMiniCard
                            className="hover:scale-[1.02] transition"
                            task={session.programmingTask}>
                        </TaskProgrammingMiniCard>
                    )}

                    {count !== undefined && isStarted && !isFinished && (
                        <h3 className="text-lg font-semibold text-primary my-3">
                            Completed task: <span className="text-foreground">{count.completed}/{count.total}</span>
                        </h3>
                    )}

                    <div className="grid grid-cols-1 sm:grid-cols-3 md:grid-cols-3 gap-3 mt-3">
                        {isEdit && !isStarted && !isFinished && (
                            <>
                                <Button
                                    className="btn-primary btn-animation flex items-center justify-center"
                                    onClick={goToTaskList}
                                >
                                    {session.programmingTask ? "Change task" : "Select task"}
                                </Button>

                                <Button
                                    className="btn-primary btn-animation flex items-center justify-center"
                                    onClick={() => navigate(`/task/ai-generate-task/null`)}
                                >
                                    Generate task
                                </Button>

                                {session.programmingTask && (
                                    <Button
                                        className="btn-green btn-animation flex items-center justify-center"
                                        onClick={() => startGame(session.id)}
                                    >
                                        Start the game
                                    </Button>
                                )}

                                <Button
                                    onClick={handlePressSelectedFriends}
                                    className="btn-primary btn-animation flex items-center justify-center"
                                >
                                    Invite friend
                                </Button>
                            </>
                        )}

                        {!activeSession && !isStarted && !isFinished ? (
                            <>
                                {isPrivate && (
                                    <InputPassword onChange={(e) => setPassword(e.target.value)} placeholder="Enter password" />
                                )}
                                <Button
                                    disabled={joinLoad}
                                    className="btn-green btn-animation flex items-center justify-center"
                                    onClick={handleJoinSession}
                                >
                                    Join
                                </Button>
                            </>

                        ) : isJoined && !isFinished && (
                            <>
                                <Button
                                    disabled={joinLoad}
                                    className="btn-red btn-animation flex items-center justify-center"
                                        onClick={leaveSession}
                                >
                                    Leave
                                </Button>
                                {isStarted && (
                                    <Button
                                            className="btn-primary btn-animation flex items-center justify-center"
                                            onClick={() => handlePlayerSessionInfo(user?.id)}
                                    >
                                        My code
                                    </Button>
                                )}

                                    {isEdit && isStarted && (
                                    <Button
                                        className="btn-green btn-animation flex items-center justify-center"
                                            onClick={() => finishGame(session.id)}
                                    >
                                        Finish game
                                    </Button>
                                )}
                            </>
                        )}
                    </div>

                    <div className="space-y-4 rounded-2xl px-4 py-3 border shadow-sm mt-3">
                        <div className="text-sm flex items-center gap-1">
                            <Users size={16} />
                            <span>
                                {players.length}/{session.maxPeople}
                            </span>
                        </div>
                        {players.length !== 0 && (
                            <PlayersList
                                players={players}
                                cardWrapperClassName="hover:scale-[1.02] transition"
                                onDelete={(!isFinished && isEdit) ? handleDeletPlayer : undefined}
                                onPlayerSessionInfo={
                                    session.status === GameStatus.InProgress
                                        ? handlePlayerSessionInfo
                                        : undefined
                                }
                            />
                        )}
                    </div>
                </div>
            </div>
            {session && (
                <EditModal open={showEditSession} title="Edit Session" onClose={() => setShowEditSession(false)}>
                    <SessionForm session={session} onClose={() => setShowEditSession(false)} onUpdate={handleUpdateSession} submitLabel="Save"></SessionForm>
                </EditModal>
            )}
            {friendships && (
                <FriendsSelectModal open={showSelectedFriends} friends={friendships} onClose={() => setShowSelectedFriends(false)} isMultipleSelect={true} onSaveSelected={handleSelectedFriend} />
            )}
        </>
    );
}

export default SessionInfo;