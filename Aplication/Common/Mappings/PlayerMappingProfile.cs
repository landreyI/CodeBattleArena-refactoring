using AutoMapper;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Friendships;
using CodeBattleArena.Domain.PlayerItems;
using CodeBattleArena.Domain.PlayerQuests;
using CodeBattleArena.Domain.Players;

namespace CodeBattleArena.Application.Common.Mappings
{
    public class PlayerMappingProfile : Profile
    {
        public PlayerMappingProfile()
        {
            CreateMap<Friendship, FriendshipDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<PlayerItem, PlayerItemDto>();
            CreateMap<PlayerQuest, PlayerQuestDto>();

            CreateMap<Player, PlayerDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Profile.Name))
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Profile.PhotoUrl))
                .ForMember(dest => dest.AdditionalInformation, opt => opt.MapFrom(src => src.Profile.AdditionalInformation))
                .ForMember(dest => dest.Victories, opt => opt.MapFrom(src => src.Stats.Victories))
                .ForMember(dest => dest.CountGames, opt => opt.MapFrom(src => src.Stats.CountGames))
                .ForMember(dest => dest.Experience, opt => opt.MapFrom(src => src.Stats.Experience))
                .ForMember(dest => dest.Coins, opt => opt.MapFrom(src => src.Wallet.Coins));
        }
    }
}
