using AutoMapper;
using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Models;

namespace CodeBattleArena.Server.Helpers.Mapping
{
    public class ItemMappingProfile : Profile
    {
        public ItemMappingProfile() {
            CreateMap<ItemDto, Item>();
            CreateMap<Item, ItemDto>();

            CreateMap<PlayerItemDto, PlayerItem>();
            CreateMap<PlayerItem, PlayerItemDto>();
        }
    }
}
