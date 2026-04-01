using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Common.Behaviours
{
    public class StaffAuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IStaffRequest
    where TResponse : Result
    {
        private readonly IIdentityService _identityService;

        public StaffAuthorizationBehavior(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            var userContext = await _identityService.GetUserContextAsync();

            if (!userContext.PlayerId.HasValue)
                return CreateFailureResponse("auth.unauthorized", "User not found in context", 401);

            if (!userContext.IsStaff)
                return CreateFailureResponse("auth.forbidden", "Only staff can do this", 403);

            return await next();
        }

        private TResponse CreateFailureResponse(string code, string message, int statusCode)
        {
            var error = new Error(code, message, statusCode);

            if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var resultType = typeof(TResponse).GetGenericArguments()[0];
                var failureMethod = typeof(Result<>).MakeGenericType(resultType)
                    .GetMethod("Failure", [typeof(Error)]);
                return (TResponse)failureMethod!.Invoke(null, [error])!;
            }

            return (TResponse)(object)Result.Failure(error);
        }
    }
}
