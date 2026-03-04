
namespace CodeBattleArena.Server.Specifications.SessionSpec
{
    public class SessionsExpiredSpec : SessionsByDaySpec
    {
        public SessionsExpiredSpec(int day, DateTime nowUtc) : base(day, nowUtc)
        {
            Criteria = s => s.WinnerId == null;
        }
    }
}
