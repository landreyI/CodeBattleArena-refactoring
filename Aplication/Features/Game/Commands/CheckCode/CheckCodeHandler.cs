
using CodeBattleArena.Application.Common.Helpers;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.Sessions.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Sessions;
using CodeBattleArena.Domain.TaskLanguages;
using CodeBattleArena.Domain.TestCases;
using MediatR;

namespace CodeBattleArena.Application.Features.Game.Commands.CheckCode
{
    public class CheckCodeHandler : IRequestHandler<CheckCodeCommand, Result<ExecutionResult>>
    {
        private readonly IRepository<Session> _sessionRepository;
        private readonly IRepository<TestCase> _testCaseRepository;
        private readonly IRepository<TaskLanguage> _taskLanguageRepository;
        private readonly IIdentityService _identityService;
        private readonly IJudge0Client _judge0Client;
        private readonly IUnitOfWork _unitOfWork;

        public CheckCodeHandler(
            IRepository<Session> sessionRepository, 
            IRepository<TestCase> testCaseRepository,
            IRepository<TaskLanguage> taskLanguageRepository,
            IIdentityService identityService,
            IJudge0Client judge0Client,
            IUnitOfWork unitOfWork)
        {
            _sessionRepository = sessionRepository;
            _testCaseRepository = testCaseRepository;
            _taskLanguageRepository = taskLanguageRepository;
            _identityService = identityService;
            _judge0Client = judge0Client;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ExecutionResult>> Handle(CheckCodeCommand request, CancellationToken ct)
        {
            var currentPlayerId = _identityService.CurrentPlayerId();
            if (!currentPlayerId.HasValue)
                return Result<ExecutionResult>.Failure(new Error("auth.unauthorized", "User not found in context", 401));

            var spec = new ActiveSessionForUpdateSpec(currentPlayerId.Value);
            var activeSession = await _sessionRepository.GetBySpecAsync(spec, ct);
            if (activeSession is null)
                return Result<ExecutionResult>.Failure(new Error("session.active", "Active session not found", 200));

            var testCases = await _testCaseRepository
                .GetListAsync(tc => tc.ProgrammingTaskId == activeSession.ProgrammingTaskId, true, ct:ct);
            var taskLanguage = await _taskLanguageRepository
                .GetFirstOrDefaultAsync(tl => tl.ProgrammingTaskId == activeSession.ProgrammingTaskId 
                && tl.ProgrammingLangId == activeSession.ProgrammingLangId, true, ct: ct);

            var testCasesDto = testCases.Select(tc => (tc.Input, tc.ExpectedOutput));
            var payload = CodeBuilder.Build(request.Code, taskLanguage.VerificationCode, testCasesDto);

            var result = await _judge0Client.CheckAsync(
                payload.source_code,
                activeSession.ProgrammingLang.ExternalId,
                payload.stdin,
                payload.expected_output,
                ct);

            var playerSession = activeSession.PlayerSessions.FirstOrDefault(ps => ps.PlayerId == currentPlayerId.Value);

            if (playerSession is null)
                return Result<ExecutionResult>.Failure(new Error("session.player_not_found", "Player is not part of this session."));

            var resultSubmit = playerSession.SubmitSolution(request.Code, result.Time, result.Memory);

            if (resultSubmit.IsFailure)
                return Result<ExecutionResult>.Failure(resultSubmit.Error);

            await _unitOfWork.CommitAsync(ct);
            return Result<ExecutionResult>.Success(result);
        }
    }
}
