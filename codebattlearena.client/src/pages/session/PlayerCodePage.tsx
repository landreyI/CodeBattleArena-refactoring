import { useNavigate, useSearchParams } from "react-router-dom";
import { usePlayerSessionInfo } from "@/hooks/session/usePlayerSessionInfo";
import EmptyState from "@/components/common/EmptyState";
import ErrorMessage from "@/components/common/ErrorMessage";
import LoadingScreen from "@/components/common/LoadingScreen";
import { useAuth } from "@/contexts/AuthContext";
import { useState, useEffect } from "react";
import { ToggleSizeButton } from "@/components/buttons/ToggleSizeButton";
import { useTask } from "@/hooks/task/useTask";
import InlineNotification from "@/components/common/InlineNotification";
import CodeViewer from "@/components/common/CodeViewer";
import InputDatasList from "@/components/lists/InputDatasList";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { formatDuration, getDifficultyColor } from "@/untils/helpers";
import { useCheckPlayerCode } from "@/hooks/playerSession/useCheckPlayerCode";
import PlayerMiniCard from "@/components/cards/PlayerMiniCard";
import { useSessionEventsHub } from "@/hooks/hubs/session/useSessionEventsHub";
import { useThrottleEffect } from "@/hooks/useThrottleEffect";
import { Eye } from "lucide-react";
import { useObserverJoin } from "@/hooks/hubs/session/useObserverJoin";
import { Card } from "@/components/ui/card";
import { ExecutionResult } from "@/models/executionResult";
import CodeVerificationResult from "@/components/cards/CodeVerificationResult";
import { useFinishTaskPlayer } from "@/hooks/playerSession/useFinishTaskPlayer";
import { ResizableHandle, ResizablePanel, ResizablePanelGroup } from "@/components/ui/resizable";
import { useSession } from "@/hooks/session/useSession";
import ScreenBlocker from "../../components/common/ScreenBlocker";
import { useSessionHubConnection } from "@/contexts/SignalRSessionHubContext";

export function PlayerCodePage() {
    const [searchParams] = useSearchParams();
    const playerId = searchParams.get("playerId");
    const sessionId = searchParams.get("sessionId");
    const { loading: checkLoad, error: checkError, checkCode } = useCheckPlayerCode();
    const { playerSession, setPlayerSession, loadPlayerSessionInfo, loading: infoLoad, error: infoError } = usePlayerSessionInfo(playerId ?? undefined, sessionId ?? undefined);
    const { session, error: sessionError } = useSession(playerSession?.finishTask ? sessionId : undefined);

    const { user } = useAuth();
    const taskId = playerSession?.session?.taskId;
    const { task, loading: taskLoad, error: taskError } = useTask(taskId ?? undefined);
    const { loading: finishLoad, error: finishError, finishTask } = useFinishTaskPlayer();
    const navigate = useNavigate();

    const connection = useSessionHubConnection();

    const [observers, setObservers] = useState<number>(0);
    const [defaultCode, setDefaultCode] = useState<string>("");
    const [code, setCode] = useState<string>("");
    const [responseCode, setResponseCode] = useState<ExecutionResult | null>(null);
    const [fullScreenPanel, setFullScreenPanel] = useState<'code' | 'task' | 'inputDatas' | null>(null);
    const [direction, setDirection] = useState<"horizontal" | "vertical">("horizontal")

    const isCurrentUserPlayer = user && user.id === playerSession?.idPlayer;
    const isEdit = isCurrentUserPlayer && !playerSession.isCompleted;

    useEffect(() => {
        const newDefaultCode = task?.preparation ?? "";
        setDefaultCode(newDefaultCode);

        // Обновляем code, только если у пользователя еще не было своего текста
        setCode(prev => prev || playerSession?.codeText || newDefaultCode);

        if (playerSession?.memory && playerSession?.time) {
            const codeVerification: ExecutionResult = {
                memory: playerSession.memory,
                time: playerSession.time,
            };
            setResponseCode(codeVerification);
        }

        const updateDirection = () => {
            setDirection(window.innerWidth < 768 ? "vertical" : "horizontal")
        }

        updateDirection() // установить начальное значение
        window.addEventListener("resize", updateDirection)
        return () => window.removeEventListener("resize", updateDirection)
    }, [task, playerSession]);

    useThrottleEffect(() => {
        const update = async () => {
            if (connection?.state === "Connected" && sessionId) {
                try {

                    await connection.invoke("UpdateCode", sessionId, code);
                } catch (err) {
                    console.error("SignalR: Failed to sync code:", err);
                }
            }
        };

        update();
    }, [code], 100);

    useObserverJoin(((!isCurrentUserPlayer) && sessionId && playerSession) ? sessionId : undefined);

    useSessionEventsHub(Number(sessionId), {
        onDelete: () => navigate("/home"),
        onLeave: (player) => {
            if (player.id === playerSession?.idPlayer)
                navigate("/home");
        },
        onUpdateCodePlayer: (code) => {
            if (user && user.id !== playerSession?.idPlayer) {
                setCode(code);
            }
        },
        onUpdatePlayerSession: (playerSessionUpdate) => {
            if (!isEdit) {
                setPlayerSession(playerSessionUpdate);
                window.scrollTo({ top: 0, behavior: 'smooth' });
            }
        },
        onUpdateObserversCount: (count) => setObservers(count),
    });

    const handleResetCode = () => setCode(defaultCode ?? "");

    const handleSubmit = async () => {
        // Post to backend here
        try {
            const data = await checkCode(code);
            setResponseCode(data);
        } catch (err) {
            console.error("Failed to update code:", err);
        }

        window.scrollTo({ top: 0, behavior: 'smooth' });
    };

    const handleFinish = async () => {
        const data = await finishTask();
        if (data)
            navigate(`/session/info-session/${sessionId}`);
    };


    if (infoLoad || taskLoad) return <LoadingScreen />

    const error = infoError || taskError;
    if (error) return <ErrorMessage error={error} />;

    if (!playerSession) return <EmptyState message="Player Session not found" />;
    if (!task) return <EmptyState message="Task not found" />;

    const errorNotifi = checkError || finishError || sessionError;

    return (
        <>
            {errorNotifi && <InlineNotification message={errorNotifi.message} className="bg-red" />}
            <ScreenBlocker open={checkLoad} />

            <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-2 mb-4">
                {playerSession.finishTask && (
                    <Card className="p-4 space-y-4 text-sm items-center">
                        Time spent writing code:{" "}<br/>
                        {playerSession?.finishTask && session?.dateStartGame
                            ? formatDuration(playerSession.finishTask, session.dateStartGame)
                            : "Not finished"}
                    </Card>
                )}
                {responseCode && (
                    <CodeVerificationResult executionResult={responseCode}></CodeVerificationResult>
                )}
                {playerSession.player && (
                    <PlayerMiniCard
                        player={playerSession.player}
                        className="hover:scale-[1.02] transition"
                    >
                    </PlayerMiniCard>
                )}
                <Card className="flex flex-row items-center gap-2 p-2">
                    <Eye size={26} />
                    <span>{observers}</span>
                </Card>
            </div>

            <ResizablePanelGroup direction={direction} key={fullScreenPanel ?? 'all'} className="flex flex-col min-h-[95vh] max-h-[95vh] gap-1">
                <ResizablePanel
                    defaultSize={fullScreenPanel === 'task' ? 100 : fullScreenPanel ? 0 : 40}
                    collapsible
                >
                    {fullScreenPanel !== 'code' && fullScreenPanel !== 'inputDatas' && (
                        <div className="h-full flex flex-col rounded-xl bg-card">
                            <div className="flex items-center justify-between p-2">
                                <Badge className={`${getDifficultyColor(task.difficulty)} `}>
                                    {task.difficulty}
                                </Badge>
                                <ToggleSizeButton
                                    fullScreen={fullScreenPanel === 'task'}
                                    onClick={() => setFullScreenPanel(fullScreenPanel === 'task' ? null : 'task')}
                                />
                            </div>
                            <div className="h-full break-words whitespace-pre-wrap overflow-y-auto bg-muted rounded-b-xl p-4">
                                {task.textTask || "No description"}
                            </div>
                        </div>
                    )}
                </ResizablePanel>

                <ResizableHandle className="border border-transparent hover:border-1 hover:border-primary hover:bg-primary" withHandle />

                <ResizablePanel defaultSize={(fullScreenPanel === 'code' || fullScreenPanel === 'inputDatas' ) ? 100 : fullScreenPanel ? 0 : 60} className="flex w-full  overflow-hidden">
                    <ResizablePanelGroup direction="vertical" className="gap-1">
                        <ResizablePanel
                            defaultSize={fullScreenPanel === 'code' ? 100 : fullScreenPanel ? 0 : 60}
                            collapsible
                            minSize={fullScreenPanel === 'task' || fullScreenPanel === 'inputDatas' ? 0 : 20}
                        >
                            {fullScreenPanel !== 'task' && fullScreenPanel !== 'inputDatas' && (
                                <div className="h-full flex flex-col bg-card rounded-xl">
                                    <div className="flex items-center justify-between p-2">
                                        <Badge className="bg-gray text-base size-6">
                                            {playerSession?.session?.langProgramming?.nameLang}
                                        </Badge>
                                        <div className="flex gap-2">
                                            {isEdit && (
                                                <Button onClick={handleResetCode} className="btn-gray size-6">
                                                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-arrow-counterclockwise" viewBox="0 0 16 16">
                                                        <path fillRule="evenodd" d="M8 3a5 5 0 1 1-4.546 2.914.5.5 0 0 0-.908-.417A6 6 0 1 0 8 2z" />
                                                        <path d="M8 4.466V.534a.25.25 0 0 0-.41-.192L5.23 2.308a.25.25 0 0 0 0 .384l2.36 1.966A.25.25 0 0 0 8 4.466" />
                                                    </svg>
                                                </Button>
                                            )}
                                            <ToggleSizeButton
                                                fullScreen={fullScreenPanel === 'code'}
                                                onClick={() => setFullScreenPanel(fullScreenPanel === 'code' ? null : 'code')}
                                            />
                                        </div>
                                    </div>
                                    <div className="h-full overflow-y-auto">
                                        <CodeViewer
                                            code={code}
                                            onChange={(val) => setCode(val)}
                                            language={task.langProgramming?.codeNameLang || "javascript"}
                                            readonly={!isEdit}
                                            autoResize={false}
                                        />
                                    </div>
                                    {isEdit && (
                                        <div className="p-2 flex justify-between">
                                            <Button onClick={handleSubmit} className="btn-primary btn-animation">Check</Button>
                                            <Button onClick={handleFinish} className="btn-green btn-animation">Finish</Button>
                                        </div>
                                    )}
                                </div>
                            )}
                        </ResizablePanel>

                        <ResizableHandle className="border border-transparent hover:border-1 hover:border-primary hover:bg-primary" withHandle />

                        <ResizablePanel
                            defaultSize={fullScreenPanel === 'inputDatas' ? 100 : fullScreenPanel ? 0 : 40}
                            collapsible
                            minSize={fullScreenPanel === 'code' || fullScreenPanel === 'task' ? 0 : 20}
                        >
                            {fullScreenPanel !== 'code' && fullScreenPanel !== 'task' && (
                                <div className="h-full flex flex-col rounded-lg bg-card">
                                    <div className="flex items-center justify-end p-2">
                                        <ToggleSizeButton
                                            fullScreen={fullScreenPanel === 'inputDatas'}
                                            onClick={() => setFullScreenPanel(fullScreenPanel === 'inputDatas' ? null : 'inputDatas')}
                                        />
                                    </div>
                                    <div className="flex-grow flex flex-col min-h-0">
                                        {task.taskInputData && (
                                            <InputDatasList inputDatas={task.taskInputData} outDatas={responseCode?.stdout} />
                                        )}
                                    </div>
                                </div>
                            )}
                        </ResizablePanel>
                    </ResizablePanelGroup>
                </ResizablePanel>
            </ResizablePanelGroup>
        </>
    );
}

export default PlayerCodePage;