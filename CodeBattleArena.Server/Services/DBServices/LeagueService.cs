using AutoMapper;
using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Helpers;
using CodeBattleArena.Server.IRepositories;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Services.DBServices.IDBServices;
using CodeBattleArena.Server.Untils;


namespace CodeBattleArena.Server.Services.DBServices
{
    public class LeagueService : ILeagueService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LeagueService> _logger;
        private readonly IMapper _mapper;
        public LeagueService(IUnitOfWork unitOfWork, ILogger<LeagueService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<PlayersLeague>> GetPlayersLeagues(CancellationToken cancellationToken)
        {
            var players = await _unitOfWork.PlayerRepository.GetPlayersAsync(null,cancellationToken);
            var leagues = await GetLeaguesAsync(cancellationToken);
            var playersLeagues = leagues
                .Select(l => new PlayersLeague(
                    league: _mapper.Map<LeagueDto>(l),
                    players: _mapper.Map<List<PlayerDto>>(players
                        .Where(p => p.Victories >= l.MinWins && p.Victories <= l.MaxWins)
                        .ToList())
                ))
                .ToList();

            return playersLeagues;
        }
        public async Task<League> GetLeagueByPlayerAsync(string idPlayer, CancellationToken cancellationToken)
        {
            return await _unitOfWork.LeagueRepository.GetLeagueByPlayerAsync(idPlayer,cancellationToken);
        }
        public async Task<League> GetLeagueByNameAsync(string name, CancellationToken cancellationToken)
        {
            return await _unitOfWork.LeagueRepository.GetLeagueByNameAsync(name, cancellationToken);
        }
        public async Task<League> GetLeagueAsync(int id, CancellationToken cancellationToken)
        {
            return await _unitOfWork.LeagueRepository.GetLeagueAsync(id, cancellationToken);
        }
        public async Task<List<League>> GetLeaguesAsync(CancellationToken cancellationToken)
        {
            var leagues = await _unitOfWork.LeagueRepository.GetLeaguesAsync(cancellationToken);
            return leagues.OrderBy(l => l.MinWins).ToList();
        }
        public async Task<Result<Unit, ErrorResponse>> AddLeagueInDbAsync
            (League league, CancellationToken cancellationToken, bool commit = true)
        {
            try
            {
                await _unitOfWork.LeagueRepository.AddLeagueAsync(league, cancellationToken);
                if (commit)
                    await _unitOfWork.CommitAsync(cancellationToken);

                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when adding League");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Database error when adding league."
                });
            }
        }
        public async Task<Result<Unit, ErrorResponse>> DeleteLeagueInDbAsync
            (int id, CancellationToken cancellationToken, bool commit = true)
        {
            try
            {
                await _unitOfWork.LeagueRepository.DeleteLeagueAsync(id, cancellationToken);
                if (commit)
                    await _unitOfWork.CommitAsync(cancellationToken);

                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when deleting League");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Database error when deleting league."
                });
            }
        }
        public async Task<Result<Unit, ErrorResponse>> UpdateLeagueInDbAsync
            (League league, CancellationToken cancellationToken, bool commit = true)
        {
            try
            {
                await _unitOfWork.LeagueRepository.UpdateLeague(league);
                if (commit)
                    await _unitOfWork.CommitAsync(cancellationToken);

                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when updating League");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Database error when updating league."
                });
            }
        }
    }
}
