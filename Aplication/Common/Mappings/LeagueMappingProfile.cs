using AutoMapper;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Leagues;

namespace CodeBattleArena.Application.Common.Mappings
{
    public class LeagueMappingProfile : Profile
    {
        public LeagueMappingProfile()
        {
           CreateMap<League, LeagueDto>();
        }
    }
}
