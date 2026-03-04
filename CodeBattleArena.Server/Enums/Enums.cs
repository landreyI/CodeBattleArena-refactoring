namespace CodeBattleArena.Server.Enums
{
    public enum SessionState
    {
        Private,
        Public
    }
    public enum Difficulty
    {
        Hard,
        Medium,
        Easy
    }
    public enum Role
    {
        Admin,
        Manager,
        Moderator,
        User,
        Banned
    }
    public enum TypeItem
    {
        Background,
        Avatar,
        Badge,
        Border,
        Title
    }

    public enum TaskType
    {
        WinCount,
        Login,
        DailyMatch,
        LeagueAdvance
    }

    public enum GameEventType
    {
        Victory,
        Defeat,
        MatchPlayed,
        LoggedInToday,
        LeagueAdvanced
    }

    public enum TaskParamKey
    {
        MinWins,            // Минимум побед для выполнения
        ResetOnLoss,        // Сброс прогресса при поражении
        RequiredLeague,     // Переход в нужную лигу
        RequiredId,         // На случай, если название будет изменяться
        MatchesPerDay,      // Матчей в день для прогресса
        DaysInRow,          // Кол-во дней подряд
        LoginRequired       // Зайти в игру
    }
}
