using AutoMapper;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Notifications;

namespace CodeBattleArena.Application.Common.Mappings
{
    public class NotificationMappingProfile : Profile
    {
        public NotificationMappingProfile()
        {
           CreateMap<Notification, NotificationDto>();
        }
    }
}
