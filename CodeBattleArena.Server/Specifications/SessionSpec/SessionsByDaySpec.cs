using CodeBattleArena.Server.Models;

namespace CodeBattleArena.Server.Specifications.SessionSpec
{
    public class SessionsByDaySpec : Specification<Session>
    {
        public SessionsByDaySpec(int day, DateTime nowUtc) 
        {
            Criteria = s => nowUtc > s.DateCreating.AddDays(day);
        }
    }
}
