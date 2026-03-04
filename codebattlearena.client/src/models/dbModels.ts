export interface Friendship {
    id: string;
    senderId: string;
    sender: Player | null;
    receiverId: string;
    receiver: Player | null;
    status: FriendshipStatus;
    createdAt: string;
    confirmedAt: string | null;
}

export interface Item {
    id: string;
    name: string;
    type: TypeItem;
    priceCoin: number | null;
    cssClass: string | null;
    imageUrl: string | null;
    description: string | null;
}

export interface League {
    id: string;
    name: string;
    photoUrl: string | null;
    minWins: number;
    maxWins: number | null;
}

export interface Player {
    id: string;
    name: string;
    photoUrl: string | null;
    additionalInformation: string | null;
    victories: number;
    countGames: number;
    experience: number;
    coins: number;
    createdAt: string;
    leagueId: string | null;
    league: League | null;
}

export interface PlayerItem {
    id: string;
    playerId: string;
    player: Player | null;
    itemId: string;
    item: Item | null;
    acquiredAt: string;
    isEquipped: boolean;
}

export interface PlayerQuest {
    id: string;
    playerId: string;
    player: Player | null;
    questId: string;
    quest: Quest | null;
    isCompleted: boolean;
    completedAt: string | null;
    isRewardClaimed: boolean;
    progressValue: string;
}

export interface PlayerSession {
    id: string;
    playerId: string;
    player: Player | null;
    sessionId: string;
    session: Session | null;
    codeText: string | null;
    time: string | null;
    memory: number | null;
    finishTask: string | null;
    isCompleted: boolean;
}

export interface ProgrammingLang {
    id: string;
    alias: string;
    name: string;
    externalId: string;
}

export interface ProgrammingTask {
    id: string;
    name: string;
    description: string;
    difficulty: Difficulty;
    taskLanguages: TaskLanguage[] | null;
    createdAt: string;
    authorId: string;
    author: Player | null;
    isGeneratedAI: boolean;
    testCases: TestCase[] | null;
}

export interface Quest {
    id: string;
    name: string;
    description: string;
    type: TaskType;
    isRepeatable: boolean;
    repeatAfterDays: number | null;
}

export interface QuestParam {
    id: string;
    questId: string;
    quest: Quest | null;
    key: string;
    value: string;
    isPrimary: boolean;
}

export interface QuestReward {
    id: string;
    questId: string;
    quest: Quest | null;
    rewardId: string;
    reward: Reward | null;
}

export interface Reward {
    id: string;
    type: string;
    amount: number;
    itemId: string | null;
    item: Item | null;
}
export interface Session {
    id: string;
    name: string;
    programmingLangId: string;
    programmingLang: ProgrammingLang | null;
    state: SessionState;
    maxPeople: number;
    timePlay: number | null;
    status: GameStatus;
    taskId: string | null;
    programmingTask: ProgrammingTask | null;
    winnerId: string | null;
    creatorId: string;
    dateCreating: string;
    dateStartGame: string | null;
    amountPeople: number;
}

export interface TaskLanguage {
    id: string;
    programmingTaskId: string;
    programmingLangId: string;
    preparation: string;
    verificationCode: string;
    isGeneratedAI: boolean;
}

export interface TestCase {
    id: string;
    programmingTaskId: string;
    input: string;
    expectedOutput: string;
    isHidden: boolean;
}

export interface LeaguePlayers {
    league?: League;
    players?: Player[];
}

export interface Message {
    idSender: string | null;
    sender: Player | null;
    messageText: string | null;
}

export enum SessionState {
    Public = "Public",
    Private = "Private"
}
export enum Role {
    Admin = "Admin",
    User = "User",
    Manager = "Manager",
    Moderator = "Moderator",
    Banned = "Banned"
}
export enum GameStatus {
    Waiting = "Waiting",
    InProgress = "InProgress",
    Finished = "Finished"
}
export enum FriendshipStatus {
    Pending = "Pending",
    Accepted = "Accepted",
    Blocked = "Blocked"
}
export enum Difficulty {
    Hard = "Hard",
    Medium = "Medium",
    Easy = "Easy"
}
export enum LeagueEnum {
    Bronze = "Bronze",
    Silver = "Silver",
    Gold = "Gold",
    Platinum = "Platinum",
    Diamond = "Diamond"
}
export enum TypeItem {
    Background = "Background",
    Avatar = "Avatar",
    Badge = "Badge",
    Border = "Border",
    Title = "Title",
}
export enum TaskType {
    WinCount = "WinCount",
    Login = "Login",
    DailyMatch = "DailyMatch",
    LeagueAdvance = "LeagueAdvance",
}
export enum TaskParamKey {
    MinWins = "MinWins",                    // Минимум побед для выполнения
    ResetOnLoss = "ResetOnLoss",            // Сброс прогресса при поражении
    RequiredLeague = "RequiredLeague",      // Переход в нужную лигу
    RequiredId = "RequiredId",              // На случай, если название будет изменяться
    MatchesPerDay = "MatchesPerDay",        // Матчей в день для прогресса
    DaysInRow = "DaysInRow",                // Кол-во дней подряд
    LoginRequired = "LoginRequired",        // Зайти в игру
}