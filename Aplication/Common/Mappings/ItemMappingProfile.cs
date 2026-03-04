using AutoMapper;
using CodeBattleArena.Application.Common.Models.Dtos;

namespace CodeBattleArena.Application.Common.Mappings
{
    public class ItemMappingProfile : Profile
    {
        public ItemMappingProfile()
        {
            CreateMap<Domain.Items.Item, ItemDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
        }
    }
}
