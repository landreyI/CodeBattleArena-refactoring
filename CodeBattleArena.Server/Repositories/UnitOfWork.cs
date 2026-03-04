using CodeBattleArena.Server.Data;
using CodeBattleArena.Server.IRepositories;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;

namespace CodeBattleArena.Server.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDBContext _context;
        private readonly UserManager<Player> _userManager;
        private readonly Lazy<ISessionRepository> _sessionRepository;
        private readonly Lazy<IPlayerRepository> _playerRepository;
        private readonly Lazy<IPlayerSessionRepository> _playerSessionRepository;
        private readonly Lazy<ITaskRepository> _taskRepository;
        private readonly Lazy<IFriendRepository> _friendRepository;
        private readonly Lazy<IChatRepository> _chatRepository;
        private readonly Lazy<ILangProgrammingRepository> _langProgrammingRepository;
        private readonly Lazy<ILeagueRepository> _leagueRepository;
        private readonly Lazy<IItemRepository> _itemRepository;
        private readonly Lazy<IPlayerItemRepository> _playerItemRepository;
        private readonly Lazy<IQuestRepository> _questRepository;
        private readonly Lazy<IStatisticsRepository> _statisticsRepository;

        public UnitOfWork(AppDBContext context, UserManager<Player> userManager,
            Lazy<ISessionRepository> sessionRepository, Lazy<IPlayerRepository> playerRepository, Lazy<IPlayerSessionRepository> playerSessionRepository,
            Lazy<ITaskRepository> taskRepository, Lazy<IFriendRepository> friendRepository, Lazy<IChatRepository> chatRepository,
            Lazy<ILangProgrammingRepository> langProgrammingRepository, Lazy<ILeagueRepository> leagueRepository, Lazy<IItemRepository> itemRepository,
            Lazy<IPlayerItemRepository> playerItemRepository, Lazy<IQuestRepository> questRepository, Lazy<IStatisticsRepository> statisticsRepository)
        {
            _context = context;
            _userManager = userManager;
            _sessionRepository = sessionRepository;
            _playerRepository = playerRepository;
            _playerSessionRepository = playerSessionRepository;
            _taskRepository = taskRepository;
            _friendRepository = friendRepository;
            _chatRepository = chatRepository;
            _langProgrammingRepository = langProgrammingRepository;
            _leagueRepository = leagueRepository;
            _itemRepository = itemRepository;
            _playerItemRepository = playerItemRepository;
            _questRepository = questRepository;
            _statisticsRepository = statisticsRepository;
        }

        public ISessionRepository SessionRepository => _sessionRepository.Value;
        public IPlayerRepository PlayerRepository => _playerRepository.Value;
        public IPlayerSessionRepository PlayerSessionRepository => _playerSessionRepository.Value;
        public ITaskRepository TaskRepository => _taskRepository.Value;
        public IFriendRepository FriendRepository => _friendRepository.Value;
        public IChatRepository ChatRepository => _chatRepository.Value;
        public ILangProgrammingRepository LangProgrammingRepository => _langProgrammingRepository.Value;
        public ILeagueRepository LeagueRepository => _leagueRepository.Value;
        public IItemRepository ItemRepository => _itemRepository.Value;
        public IPlayerItemRepository PlayerItemRepository => _playerItemRepository.Value;
        public IQuestRepository QuestRepository => _questRepository.Value;
        public IStatisticsRepository StatisticsRepository => _statisticsRepository.Value;

        public async Task<int> CommitAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}