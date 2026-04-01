
using CodeBattleArena.Application.Features.Items.Filters;
using CodeBattleArena.Application.Features.Notifications.Filters;
using CodeBattleArena.Application.Features.ProgrammingTasks.Filters;
using CodeBattleArena.Application.Features.Sessions.Filters;

namespace CodeBattleArena.Application.Common
{
    public static class CacheKeys
    {
        private const string Version = "v1";

        public static class Tasks
        {
            public const string Prefix = "tasks";
            public const string ListTag = $"{Version}:{Prefix}:tag:list";
            public const string AllTag = $"{Version}:{Prefix}:tag:all";

            public static string Details(Guid id) => $"{Version}:{Prefix}:details:{id}";

            public static string List(ProgrammingTaskFilter? filter)
            {
                // Возвращаем уникальный ключ для списка без фильтров
                if (filter == null) return $"{Version}:{Prefix}:list:all";

                return $"{Version}:{Prefix}:list:p{filter.PageNumber}:s{filter.PageSize}:" +
                       $"l{filter.IdLang?.ToString() ?? "any"}:" +
                       $"d{filter.Difficulty ?? "any"}";
            }

            public static string PlayerList(Guid playerId, ProgrammingTaskFilter? filter)
            {
                if (filter == null) return $"{Version}:{Prefix}:list:all:player:{playerId}";

                return $"{Version}:{Prefix}:list:player:{playerId}:p{filter.PageNumber}:s{filter.PageSize}:" +
                       $"l{filter.IdLang?.ToString() ?? "any"}:" +
                       $"d{filter.Difficulty ?? "any"}";
            }
        }

        public static class Sessions
        {
            public const string Prefix = "sessions";
            public const string ListTag = $"{Version}:{Prefix}:tag:list";
            public const string AllTag = $"{Version}:{Prefix}:tag:all";

            public static string Details(Guid id) => $"{Version}:{Prefix}:details:{id}";
            public static string Presence(Guid id) => $"{Version}:{Prefix}:presence:{id}";

            public static string List(SessionFilter? filter)
            {
                if (filter == null) return $"{Version}:{Prefix}:list:all";

                return $"{Version}:{Prefix}:list:p{filter.PageNumber}:s{filter.PageSize}:" +
                       $"l{filter.IdLang?.ToString() ?? "any"}:" +
                       $"m{filter.MaxPeople ?? 10}:" +
                       $"ss{filter.SessionState ?? "any"}:" +
                       $"st{filter.Status ?? "any"}";
            }
            public static string PlayerList(Guid playerId, SessionFilter? filter)
            {
                if (filter == null) return $"{Version}:{Prefix}:list:all:player:{playerId}";

                return $"{Version}:{Prefix}:list:player:{playerId}:p{filter.PageNumber}:s{filter.PageSize}:" +
                       $"l{filter.IdLang?.ToString() ?? "any"}:" +
                       $"m{filter.MaxPeople ?? 10}:" +
                       $"ss{filter.SessionState ?? "any"}:" +
                       $"st{filter.Status ?? "any"}";
            }
        }

        public static class Items
        {
            public const string Prefix = "items";
            public const string ListTag = $"{Version}:{Prefix}:tag:list";
            public const string AllTag = $"{Version}:{Prefix}:tag:all";

            public static string Details(Guid id) => $"{Version}:{Prefix}:details:{id}";

            public static string List(ItemFilter? filter)
            {
                if (filter == null) return $"{Version}:{Prefix}:list:all";

                return $"{Version}:{Prefix}:list:p{filter.PageNumber}:s{filter.PageSize}:" +
                       $"t{filter.Type ?? "any"}:" +
                       $"c{filter.Coin ?? 100000}" +
                       $"c{filter.IsCoinDescending ?? true}";
            }
            public static string PlayerList(Guid playerId, ItemFilter? filter)
            {
                if (filter == null) return $"{Version}:{Prefix}:list:all:player:{playerId}";

                return $"{Version}:{Prefix}:list:player:{playerId}:p{filter.PageNumber}:s{filter.PageSize}:" +
                       $"t{filter.Type ?? "any"}:" +
                       $"c{filter.Coin ?? 100000}:" +
                       $"c{filter.IsCoinDescending ?? true}";
            }
        }

        public static class ProgrammingLanguage
        {
            public const string Prefix = "languages";
            public const string ListTag = $"{Version}:{Prefix}:tag:list";
            public const string AllTag = $"{Version}:{Prefix}:tag:all";

            public static string List()
            {
                return $"{Version}:{Prefix}:list:all";
            }
        }

        public static class Notification
        {
            public const string Prefix = "notifications";
            public const string ListTag = $"{Version}:{Prefix}:tag:list";
            public const string AllTag = $"{Version}:{Prefix}:tag:all";

            public static string PlayerList(Guid playerId, NotificationFilter? filter)
            {
                if (filter == null) return $"{Version}:{Prefix}:list:all:player:{playerId}";

                return $"{Version}:{Prefix}:list:player:{playerId}:p{filter.PageNumber}:s{filter.PageSize}:" +
                       $"t{filter.Type ?? "any"}";
            }
        }

        public static class Leagues
        {
            public const string Prefix = "leagues";
            public const string ListTag = $"{Version}:{Prefix}:tag:list";

            public const string AllTag = $"{Version}:{Prefix}:tag:all";

            public static string Details(Guid id) => $"{Version}:{Prefix}:details:{id}";
            public static string PlayerLeague(Guid palyerId) => $"{Version}:{Prefix}:player:league:{palyerId}";
            public static string List() => $"{Version}:{Prefix}:list:simple";

            public static string PlayersList() => $"{Version}:{Prefix}:list:players";
        }
    }
}
