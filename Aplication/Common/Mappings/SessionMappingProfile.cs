using AutoMapper;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.PlayerSessions;
using CodeBattleArena.Domain.Sessions;

namespace CodeBattleArena.Application.Common.Mappings
{
    public class SessionMappingProfile : Profile
    {
        public SessionMappingProfile()
        {
            CreateMap<Session, SessionDto>()
                // Мапим данные напрямую из твоего рекорда SessionAccess
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.Access.Type.ToString()))
                .ForMember(dest => dest.MaxPeople, opt => opt.MapFrom(src => src.Access.MaxPeople))

                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.AmountPeople, opt => opt.MapFrom(src => src.PlayerSessions.Count));

            CreateMap<PlayerSession, PlayerSessionDto>()
                .ForMember(dest => dest.CodeText, opt => opt.MapFrom(src => src.Result.CodeText))
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Result.Time))
                .ForMember(dest => dest.Memory, opt => opt.MapFrom(src => src.Result.Memory))
                .ForMember(dest => dest.FinishTask, opt => opt.MapFrom(src => src.Result.FinishTask));

            CreateMap<PlayerSession, PlayerDto>()
                .ConvertUsing((src, dest, context) => context.Mapper.Map<PlayerDto>(src.Player));
        }
    }
}
