using AutoMapper;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.ProgrammingLanguages;

namespace CodeBattleArena.Application.Common.Mappings
{
    public class LangProgrammingMappingProfile : Profile
    {
        public LangProgrammingMappingProfile()
        {
            CreateMap<ProgrammingLang, ProgrammingLangDto>();
        }
    }
}
