import { lazy } from "react";

export const routes = [
    // Основные страницы
    {
        path: "/",
        element: lazy(() => import("@/pages/HomePage")),
        index: true,
    },
    {
        path: "/home",
        element: lazy(() => import("@/pages/HomePage")),
    },
    {
        path: "/info",
        element: lazy(() => import("@/pages/InfoPage")),
    },
    {
        path: "/login-error",
        element: lazy(() => import("@/pages/LoginError")),
    },
    {
        path: "/google-oauth-success",
        element: lazy(() => import("@/pages/GoogleOauthSuccess")),
    },

    // Игроки
    {
        path: "/player/info-player/:playerId",
        element: lazy(() => import("@/pages/player/PlayerPage")),
    },
    {
        path: "/player/list-players",
        element: lazy(() => import("@/pages/player/PlayersListPage")),
    },

    // Друзья
    {
        path: "/friend/list-friends",
        element: lazy(() => import("@/pages/friend/FriendsListPage")),
    },

    // Сессии
    {
        path: "/session/info-session/:sessionId",
        element: lazy(() => import("@/pages/session/SessionInfo")),
    },
    {
        path: "/session/list-sessions",
        element: lazy(() => import("@/pages/session/SessionsList")),
    },
    {
        path: "/session/create-session",
        element: lazy(() => import("@/pages/session/CreateSession")),
    },
    {
        path: "/session/player-code",
        element: lazy(() => import("@/pages/session/PlayerCodePage")),
    },

    // Задачи
    {
        path: "/task/list-task",
        element: lazy(() => import("@/pages/task/TasksListPage")),
    },
    {
        path: "/task/info-task/:taskId",
        element: lazy(() => import("@/pages/task/TaskInfo")),
    },
    {
        path: "/task/create-task",
        element: lazy(() => import("@/pages/task/CreateTask")),
    },
    {
        path: "/task/my-tasks/:playerId",
        element: lazy(() => import("@/pages/task/PlayerTasksPage")),
    },
    {
        path: "/task/ai-generate-task/:taskId",
        element: lazy(() => import("@/pages/task/AIGenerateTask")),
    },

    // Лиги
    {
        path: "/league/list-leagues",
        element: lazy(() => import("@/pages/league/LeaguesPage")),
    },
    {
        path: "/league/create-league",
        element: lazy(() => import("@/pages/league/CreateLeague")),
    },

    // Предметы
    {
        path: "/item/info-item/:itemId",
        element: lazy(() => import("@/pages/item/ItemInfo")),
    },
    {
        path: "/item/player-items/:playerId",
        element: lazy(() => import("@/pages/item/PlayerItemsPage")),
    },
    {
        path: "/item/list-items",
        element: lazy(() => import("@/pages/item/ItemsListPage")),
    },
    {
        path: "/item/create-item",
        element: lazy(() => import("@/pages/item/CreateItem")),
    },

    // Квесты и награды
    {
        path: "/quest/info-quest/:taskPlayId",
        element: lazy(() => import("@/pages/quest/QuestPage")),
    },
    {
        path: "/quest/list-quests",
        element: lazy(() => import("@/pages/quest/QuestsListPage")),
    },
    {
        path: "/quest/create-quest",
        element: lazy(() => import("@/pages/quest/CreateQuest")),
    },
    {
        path: "/quest/list-rewards",
        element: lazy(() => import("@/pages/quest/RewardsListPage")),
    },
    {
        path: "/quest/create-reward",
        element: lazy(() => import("@/pages/quest/CreateReward")),
    },

    // Statistics
    {
        path: "/statistics/info-statistics",
        element: lazy(() => import("@/pages/statistics/StatisticsPage")),
    },
];
