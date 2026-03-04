using CodeBattleArena.Application.Features.Auth.Models;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Auth.Commands.RefreshToken
{
    public record RefreshTokenCommand(string refreshToken) : IRequest<Result<AuthResponse>>;
}
