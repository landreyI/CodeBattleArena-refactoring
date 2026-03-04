using CodeBattleArena.Application.Features.Auth.Models;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Auth.Commands.Login
{
    public class LoginCommand : IRequest<Result<AuthResponse>>
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
