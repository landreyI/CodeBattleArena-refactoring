using CodeBattleArena.Application.Features.Auth.Models;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Auth.Commands.GoogleLogin
{
    public record GoogleLoginCommand(string Code) : IRequest<Result<AuthResponse>>;
}
