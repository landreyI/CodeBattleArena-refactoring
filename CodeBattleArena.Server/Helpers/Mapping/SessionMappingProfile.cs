using AutoMapper;
using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Models;

namespace CodeBattleArena.Server.Helpers.Mapping
{
    public class SessionMappingProfile : Profile
    {
        public SessionMappingProfile()
        {
            CreateMap<SessionDto, Session>()
                .ForMember(dest => dest.LangProgramming, opt => opt.Ignore())
                .ForMember(dest => dest.TaskProgramming, opt => opt.Ignore());

            CreateMap<Session, SessionDto>()
                .ForMember(dest => dest.IdSession, opt => opt.MapFrom(src => src.IdSession))
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.AmountPeople, opt => opt.MapFrom(src => src.PlayerSessions.Count));
        }
    }
}
