using AutoMapper;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Players;
using Microsoft.AspNetCore.SignalR;

namespace CodeBattleArena.Infrastructure.SignalR.Hubs
{
    public class SessionHub : Hub
    {
        private readonly ICacheService _cacheService;
        private readonly IIdentityService _identityService;
        private readonly IRepository<Player> _playerRepository;
        private readonly IMapper _mapper;

        private readonly string _observersPrefix = "session_observers:";
        private readonly string _connectionToSessionPrefix = "observer_at:";

        public SessionHub(
            ICacheService cacheService,
            IIdentityService identityService,
            IRepository<Player> playerRepository,
            IMapper mapper)
        {
            _cacheService = cacheService;
            _identityService = identityService;
            _playerRepository = playerRepository;
            _mapper = mapper;
        }
        public async Task JoinSessionGroup(string sessionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Session-{sessionId}");
        }

        public async Task LeaveSessionGroup(string sessionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Session-{sessionId}");
        }

        public async Task JoinAsObserver(string sessionId)
        {
            // Добавляем в Set сессии в Redis (SADD)
            await _cacheService.AddToSetAsync(_observersPrefix + sessionId, Context.ConnectionId);

            // Запоминаем, какую сессию смотрит этот ConnectionId (для дисконнекта)
            await _cacheService.SetAsync(_connectionToSessionPrefix + Context.ConnectionId, sessionId, TimeSpan.FromHours(2));

            await Groups.AddToGroupAsync(Context.ConnectionId, $"Session-{sessionId}");

            // Получаем актуальное количество (SCARD)
            var count = await _cacheService.GetSetCountAsync(_observersPrefix + sessionId);

            await Clients.Group($"Session-{sessionId}")
                         .SendAsync("UpdateObserversCount", count);
        }

        public async Task LeaveAsObserver(string sessionId)
        {
            // Удаляем из Set (SREM)
            await _cacheService.RemoveFromSetAsync(_observersPrefix + sessionId, Context.ConnectionId);
            await _cacheService.RemoveAsync(_connectionToSessionPrefix + Context.ConnectionId);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Session-{sessionId}");

            var count = await _cacheService.GetSetCountAsync(_observersPrefix + sessionId);

            await Clients.Group($"Session-{sessionId}")
                         .SendAsync("UpdateObserversCount", count);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Пытаемся узнать, какую сессию наблюдал пользователь
            var sessionId = await _cacheService.GetAsync<string>(_connectionToSessionPrefix + Context.ConnectionId);

            if (!string.IsNullOrEmpty(sessionId))
            {
                // Удаляем его из списка наблюдателей сессии
                await _cacheService.RemoveFromSetAsync(_observersPrefix + sessionId, Context.ConnectionId);
                await _cacheService.RemoveAsync(_connectionToSessionPrefix + Context.ConnectionId);

                var count = await _cacheService.GetSetCountAsync(_observersPrefix + sessionId);

                // Уведомляем группу об изменении счетчика
                await Clients.Group($"Session-{sessionId}")
                             .SendAsync("UpdateObserversCount", count);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task UpdateCode(Guid sessionId, string code)
        {
            // Рассылаем всем в группе сессии, кроме отправителя
            await Clients.OthersInGroup($"Session-{sessionId}").SendAsync("UpdateCodePlayer", code);
        }

        public async Task SendMessage(Guid sessionId, string message)
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(message)) return;

            var player = await _playerRepository.GetFirstOrDefaultAsync(p => p.IdentityId == userId, true);
            var playerDto = _mapper.Map<PlayerDto>(player);

            var messageDto = new MessageDto
            {
                IdSender = player.Id,
                Sender = playerDto,
                MessageText = message,
            };

            await Clients.Group($"Session-{sessionId}").SendAsync("SendMessageSession", messageDto);
        }
    }
}
