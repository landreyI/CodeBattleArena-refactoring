using CodeBattleArena.Server.Data;
using CodeBattleArena.Server.IRepositories;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Specifications;
using CodeBattleArena.Server.Specifications.PlayerSessionSpec;
using Microsoft.EntityFrameworkCore;

namespace CodeBattleArena.Server.Repositories
{
    public class PlayerSessionRepository : IPlayerSessionRepository
    {
        private readonly AppDBContext _context;

        public PlayerSessionRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task AddPlayerSessionAsync(PlayerSession playerSession, CancellationToken cancellationToken)
        {
            await _context.PlayersSession.AddAsync(playerSession);
        }
        public async Task<PlayerSession> GetPlayerSessionAsync(ISpecification<PlayerSession> spec, CancellationToken cancellationToken)
        {
            var query = _context.PlayersSession.AsQueryable();
            query = SpecificationEvaluator.GetQuery(query, spec);
            return await query.FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<List<PlayerSession>> GetListPlayerSessionByIdAsync(ISpecification<PlayerSession> spec, CancellationToken cancellationToken)
        {
            var query = _context.PlayersSession.AsQueryable();
            query = SpecificationEvaluator.GetQuery(query, spec);
            return await query.ToListAsync(cancellationToken);
        }
        public Task UpdatePlayerSession(PlayerSession playerSession)
        {
            _context.PlayersSession.Update(playerSession);
            return Task.CompletedTask;
        }
        public async Task FinishTaskAsync(int idSession, string idPlayer, CancellationToken cancellationToken)
        {
            var playerSession = await GetPlayerSessionAsync(new PlayerSessionByIdSpec(idSession, idPlayer), cancellationToken);
            playerSession.FinishTask = DateTime.UtcNow;
            if (playerSession != null) playerSession.IsCompleted = true;
        }
        public async Task DelPlayerSessionAsync(int idSession, string idPlayer, CancellationToken cancellationToken)
        {
            var playerSession = await GetPlayerSessionAsync(new PlayerSessionByIdSpec(idSession, idPlayer), cancellationToken);
            if (playerSession != null) _context.PlayersSession.Remove(playerSession);

        }
    }
}
