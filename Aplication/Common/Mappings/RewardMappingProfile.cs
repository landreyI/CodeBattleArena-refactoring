using AutoMapper;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Rewards;

namespace CodeBattleArena.Application.Common.Mappings
{
    public class RewardMappingProfile : Profile
    {
        public RewardMappingProfile() 
        {
            CreateMap<Reward, RewardDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
        }
    }
}
