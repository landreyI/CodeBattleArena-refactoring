
using CodeBattleArena.Application.Common.Contracts;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.ProgrammingTasks.Commands.RequestAIProgrammingTask;
using CodeBattleArena.Domain.Common;
using MassTransit;
using MediatR;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Commands.GenerateAI
{
    public class RequestAIProgrammingTaskHandler : IRequestHandler<RequestAIProgrammingTaskCommand, Result<Guid>>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IIdentityService _identityService;

        public RequestAIProgrammingTaskHandler(IPublishEndpoint publishEndpoint, IIdentityService identityService)
        {
            _publishEndpoint = publishEndpoint;
            _identityService = identityService;
        }
        public async Task<Result<Guid>> Handle(RequestAIProgrammingTaskCommand request, CancellationToken ct)
        {
            Guid jobId = Guid.NewGuid();

            var user = await _identityService.GetUserContextAsync();
            if (!user.IsAuthenticated)
                return Result<Guid>.Failure(new Error("auth.unauthorized", "User not found", 401));

            await _publishEndpoint.Publish(new GenerateAITaskMessage
            {
                JobId = jobId,
                ExistingId = request.ExistingId,
                ProgrammingLangIds = request.ProgrammingLangIds,
                Prompt = request.Prompt,
                Difficulty = request.Difficulty,
                UserId = user.PlayerId!.Value
            }, ct);

            return Result<Guid>.Success(jobId);
        }
    }
}
