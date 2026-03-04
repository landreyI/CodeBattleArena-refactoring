
using CodeBattleArena.Server.Models;
using System;

namespace CodeBattleArena.Server.Specifications.SessionSpec
{
    public class SessionsFinishExpiredSpec : Specification<Session>
    {
        public SessionsFinishExpiredSpec(DateTime nowUtc)
        {
            Criteria = s => !s.IsFinish &&
                    s.DateStartGame != null &&
                    s.TimePlay != null &&
                    s.DateStartGame.Value.AddMinutes((double)s.TimePlay.Value) <= nowUtc;
        }
    }
}
