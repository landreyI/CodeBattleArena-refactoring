
namespace CodeBattleArena.Application.Features.ProgrammingTasks.Filters
{
    public class ProgrammingTaskFilter
    {
        public Guid? IdLang { get; set; }
        public string? Difficulty { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
    }
}
