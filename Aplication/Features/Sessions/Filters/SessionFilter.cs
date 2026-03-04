
namespace CodeBattleArena.Application.Features.Sessions.Filters
{
    public class SessionFilter
    {
        public Guid? IdLang { get; set; }
        public int? MaxPeople { get; set; }
        public string? SessionState { get; set; }
        public string? Status { get; set; }
    }
}
