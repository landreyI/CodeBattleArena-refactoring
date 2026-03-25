
using AutoMapper;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.ProgrammingLanguages;
using MediatR;

namespace CodeBattleArena.Application.Features.ProgrammingLanguages.Queries.GetLanguagesList
{
    public class GetLanguagesListHandler : IRequestHandler<GetLanguagesListQuery, Result<IReadOnlyList<ProgrammingLangDto>>>
    {
        private readonly IRepository<ProgrammingLang> _langRepository;
        private readonly IMapper _mapper;
        public GetLanguagesListHandler(IRepository<ProgrammingLang> langRepository, IMapper mapper)
        {
            _langRepository = langRepository;
            _mapper = mapper;
        }
        public async Task<Result<IReadOnlyList<ProgrammingLangDto>>> Handle(GetLanguagesListQuery request, CancellationToken cancellationToken)
        {
            var langs = await _langRepository.GetListAsync(null, ct: cancellationToken);

            return Result<IReadOnlyList<ProgrammingLangDto>>.Success(_mapper.Map<IReadOnlyList<ProgrammingLangDto>>(langs));
        }
    }
}
