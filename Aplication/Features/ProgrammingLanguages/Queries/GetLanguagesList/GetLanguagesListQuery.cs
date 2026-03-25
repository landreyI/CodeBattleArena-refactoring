
using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.ProgrammingLanguages.Queries.GetLanguagesList
{
    public record GetLanguagesListQuery() : IRequest<Result<IReadOnlyList<ProgrammingLangDto>>>, ICachableRequest
    {
        public string CacheKey => CacheKeys.ProgrammingLanguage.List();
        public TimeSpan? Expiration => TimeSpan.FromMinutes(20);
        public string[] Tags => [CacheKeys.ProgrammingLanguage.ListTag];
    }
}
