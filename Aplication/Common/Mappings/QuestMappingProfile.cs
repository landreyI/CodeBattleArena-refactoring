using AutoMapper;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.QuestParams;
using CodeBattleArena.Domain.QuestRewards;
using CodeBattleArena.Domain.Quests;

namespace CodeBattleArena.Application.Common.Mappings
{
    public class QuestMappingProfile : Profile
    {
        public QuestMappingProfile() 
        {
            CreateMap<QuestReward, QuestRewardDto>();
            CreateMap<QuestParam, QuestParamDto>()
                .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.Key.ToString()));

            CreateMap<Quest, QuestDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.IsRepeatable, opt => opt.MapFrom(src => src.Recurrence.IsRepeatable))
                .ForMember(dest => dest.RepeatAfterDays, opt => opt.MapFrom(src => src.Recurrence.RepeatAfterDays));
        }
    }
}
