using AutoMapper;
using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Models;

namespace CodeBattleArena.Server.Helpers.Mapping
{
    public class TaskPlayMappingProfile : Profile
    {
        public TaskPlayMappingProfile() 
        {
            CreateMap<TaskPlayDto, TaskPlay>();
            CreateMap<TaskPlay, TaskPlayDto>();

            CreateMap<TaskPlayParamDto, TaskPlayParam>();
            CreateMap<TaskPlayParam, TaskPlayParamDto>();

            CreateMap<TaskPlayParamDto, TaskPlayParam>();
            CreateMap<TaskPlayParam, TaskPlayParamDto>();

            CreateMap<RewardDto, Reward>();
            CreateMap<Reward, RewardDto>();

            CreateMap<TaskPlayRewardDto, TaskPlayReward>();
            CreateMap<TaskPlayReward, TaskPlayRewardDto>();

            CreateMap<PlayerTaskPlayDto, PlayerTaskPlay>();
            CreateMap<PlayerTaskPlay, PlayerTaskPlayDto>();
        }
    }
}
