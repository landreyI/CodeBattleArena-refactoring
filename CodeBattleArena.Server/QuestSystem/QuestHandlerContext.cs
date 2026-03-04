using CodeBattleArena.Server.QuestSystem.Handlers;
using CodeBattleArena.Server.QuestSystem.Interfaces;
using CodeBattleArena.Server.Services.DBServices;
using CodeBattleArena.Server.Services.DBServices.IDBServices;

namespace CodeBattleArena.Server.QuestSystem
{
    public class QuestHandlerContext
    {
        private readonly IQuestService _questService;
        private readonly ILeagueService _leagueService;

        private readonly Lazy<WinCountQuestHandler> _winCount;
        private readonly Lazy<RequiredLeagueQuestHandler> _requiredLeague;
        private readonly Lazy<MatchPlayedQuestHandler> _matchPlayed;

        public QuestHandlerContext(IQuestService questService, ILeagueService leagueService)
        {
            _questService = questService;
            _leagueService = leagueService;

            _winCount = new Lazy<WinCountQuestHandler>(() => new WinCountQuestHandler(_questService));
            _requiredLeague = new Lazy<RequiredLeagueQuestHandler>(() => new RequiredLeagueQuestHandler(_questService, _leagueService));
            _matchPlayed = new Lazy<MatchPlayedQuestHandler>(() => new MatchPlayedQuestHandler(_questService));
        }

        public IEnumerable<IQuestTriggerHandler> Handlers => new IQuestTriggerHandler[]
        {
            _winCount.Value,
            _requiredLeague.Value,
            _matchPlayed.Value,
        };
    }
}
