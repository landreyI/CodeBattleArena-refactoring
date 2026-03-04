using CodeBattleArena.Server.Enums;
using CodeBattleArena.Server.Models;

namespace CodeBattleArena.Server.Filters
{
    public class TaskProgrammingFilter : IFilter<TaskProgramming>
    {
        public string? IdPlayer { get; set; }
        public int? IdLang { get; set; }
        public Difficulty? Difficulty { get; set; }

        public IQueryable<TaskProgramming> ApplyTo(IQueryable<TaskProgramming> query)
        {
            if (!string.IsNullOrEmpty(IdPlayer))
                query = query.Where(x => x.IdPlayer == IdPlayer);

            if (Difficulty.HasValue)
                query = query.Where(x => x.Difficulty == Difficulty.Value);

            if (IdLang.HasValue)
                query = query.Where(x => x.LangProgrammingId == IdLang);

            return query;
        }

        public bool IsEmpty()
        {
            return !IdLang.HasValue && !Difficulty.HasValue;
        }
    }
}
