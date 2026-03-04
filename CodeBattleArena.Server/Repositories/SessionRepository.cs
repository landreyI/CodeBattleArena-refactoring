using CodeBattleArena.Server.Data;
using CodeBattleArena.Server.Enums;
using CodeBattleArena.Server.Filters;
using CodeBattleArena.Server.IRepositories;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Specifications;
using CodeBattleArena.Server.Specifications.SessionSpec;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CodeBattleArena.Server.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly AppDBContext _context;

        public SessionRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task StartGameAsync(int idSession, CancellationToken cancellationToken)
        {
            var session = await _context.Sessions.FindAsync(idSession, cancellationToken);
            if (session != null)
            {
                session.DateStartGame = DateTime.UtcNow;
                session.IsStart = true;
                _context.Sessions.Update(session);
            }
        }
        public async Task FinishGameAsync(int idSession, CancellationToken cancellationToken)
        {
            var session = await _context.Sessions.FindAsync(idSession, cancellationToken);
            if (session != null)
            {
                session.IsFinish = true;
                _context.Sessions.Update(session);
            }
        }
        public async Task<PlayerSession> GetVinnerAsync(int idSession, CancellationToken cancellationToken)
        {
            var session = await _context.Sessions.FindAsync(idSession, cancellationToken);
            if (session != null && session.WinnerId != null)
                return await _context.PlayersSession
                    .Include(p => p.Player)
                    .FirstOrDefaultAsync(p => p.IdSession == idSession && p.IdPlayer == session.WinnerId, cancellationToken);

            return null;
        }
        public async Task AddSessionAsync(Session session, CancellationToken cancellationToken)
        {
            session.DateCreating = DateTime.UtcNow;
            await _context.Sessions.AddAsync(session);
        }
        public async Task AddTaskToSession(int idSession, int idTask, CancellationToken cancellationToken)
        {
            var session = await _context.Sessions
                .FirstOrDefaultAsync(s => s.IdSession == idSession, cancellationToken);

            var taskProgramming = await _context.TasksProgramming
                .FirstOrDefaultAsync(t => t.IdTaskProgramming == idTask, cancellationToken);

            if (taskProgramming != null && session != null) session.TaskId = idTask;
        }
        public async Task<Session> GetSessionAsync(ISpecification<Session> spec, CancellationToken cancellationToken)
        {
            var query = _context.Sessions.AsQueryable();
            query = SpecificationEvaluator.GetQuery(query, spec);
            return await query.FirstOrDefaultAsync(cancellationToken);
        }
        public async Task ChangePasswordSessionAsync(int idSession, string password, CancellationToken cancellationToken)
        {
            var session = await _context.Sessions
                .FirstOrDefaultAsync(s => s.IdSession == idSession, cancellationToken);
            if (session != null && !string.IsNullOrEmpty(password)) session.Password = password;
        }
        public async Task DelSessionAsync(int id, CancellationToken cancellationToken)
        {
            var session = await _context.Sessions.FindAsync(id, cancellationToken);
            if (session != null) _context.Sessions.Remove(session);
        }
        public async Task DelTaskToSession(int idSession, CancellationToken cancellationToken)
        {
            var session = await _context.Sessions.FindAsync(idSession, cancellationToken);
            if (session != null)
            {
                session.TaskProgramming = null;
                session.TaskId = null;
            }
        }
        public async Task<List<Player>> GetListPlayerFromSessionAsync(int idSession, CancellationToken cancellationToken)
        {
            return await _context.PlayersSession
                .Where(s => s.IdSession == idSession)
                .Select(ps => ps.Player)
                .ToListAsync(cancellationToken);
        }
        public async Task<int> GetPlayerCountInSessionAsync(int idSession, CancellationToken cancellationToken)
        {
            return await _context.PlayersSession.CountAsync(p => p.IdSession == idSession, cancellationToken);
        }
        public async Task<List<Session>> GetListSessionAsync(ISpecification<Session> spec, CancellationToken cancellationToken)
        {
            var query = _context.Sessions
                .AsQueryable();

            return await SpecificationEvaluator.GetQuery(query, spec)
                .ToListAsync(cancellationToken);
        }
        public async Task<List<int>> DeleteExpiredSessionsAsync(DateTime dateTime, CancellationToken cancellationToken)
        {
            // Выборка сессий, которые существуют более одного дня
            var query = _context.Sessions.AsQueryable().Where(s => !s.IsFinish);
            var listSessionsExpired = await SpecificationEvaluator
                .GetQuery(query, new SessionsByDaySpec(1, dateTime))
                .ToListAsync(cancellationToken);

            if (listSessionsExpired.Any())
                _context.Sessions.RemoveRange(listSessionsExpired);

            return listSessionsExpired.Select(s => s.IdSession).ToList();
        }
        public async Task<List<int>> FinishExpiredSessionsAsync(DateTime dateTime, CancellationToken cancellationToken)
        {
            var query = _context.Sessions.AsQueryable();
            var expiredSessions = await SpecificationEvaluator
                .GetQuery(query, new SessionsFinishExpiredSpec(dateTime))
                .ToListAsync(cancellationToken);

            foreach (var session in expiredSessions)
            {
                session.IsFinish = true;
            }

            _context.Sessions.UpdateRange(expiredSessions);
            return expiredSessions.Select(s => s.IdSession).ToList();
        }

        public Task UpdateSession(Session session)
        {
            _context.Sessions.Update(session);
            return Task.CompletedTask;
        }
    }
}
