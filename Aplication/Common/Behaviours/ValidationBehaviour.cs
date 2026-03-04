
using CodeBattleArena.Domain.Common;
using FluentValidation;
using MediatR;

namespace CodeBattleArena.Application.Common.Behaviours
{
    public class ValidationBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, ct)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Any())
            {
                // Собираем словарь: Ключ = Поле, Значение = Список твоих новых кодов
                var errorsMap = failures
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key.ToLower(), // Имя поля (например, "email")
                        g => g.Select(e =>
                        {
                            // e.PropertyName - это "Email", e.ErrorCode - это "notempty" (из резолвера выше)
                            return $"{e.PropertyName.ToLower()}.{e.ErrorCode}";
                        }).ToArray()
                    );

                // Возвращаем Result.Failure с этим словарем
                return CreateFailureResponse(errorsMap);
            }

            return await next();
        }

        private TResponse CreateFailureResponse(Dictionary<string, string[]> errors)
        {
            var error = new Error("validation_error", "One or more validation errors occurred", 400);

            var resultType = typeof(TResponse);

            var failureMethod = resultType.GetMethod("Failure", new[] { typeof(Error), typeof(Dictionary<string, string[]>) });

            if (failureMethod != null)
            {
                return (TResponse)failureMethod.Invoke(null, new object[] { error, errors })!;
            }

            var fallbackMethod = resultType.GetMethod("Failure", new[] { typeof(Error) });
            return (TResponse)fallbackMethod?.Invoke(null, new object[] { error })!;
        }
    }
}
