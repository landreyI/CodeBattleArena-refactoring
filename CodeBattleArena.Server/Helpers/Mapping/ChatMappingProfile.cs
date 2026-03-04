using AutoMapper;
using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Models;

namespace CodeBattleArena.Server.Helpers.Mapping
{
    public class ChatMappingProfile : Profile
    {
        public ChatMappingProfile()
        {
            CreateMap<ChatDto, Chat>();
            CreateMap<Chat, ChatDto>();

            CreateMap<MessageDto, Message>();
            CreateMap<Message, MessageDto>();
        }
    }
}
