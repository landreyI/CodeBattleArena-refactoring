using AutoMapper;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.ProgrammingTasks;
using CodeBattleArena.Domain.TaskLanguages;
using CodeBattleArena.Domain.TestCases;

namespace CodeBattleArena.Application.Common.Mappings
{
    public class ProgrammingTaskMappingProfile : Profile
    {
        public ProgrammingTaskMappingProfile()
        {
            CreateMap<TestCase, TestCaseDto>();

            CreateMap<TaskLanguage, TaskLanguageDto>();

            CreateMap<ProgrammingTask, ProgrammingTaskDto>()
                .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => src.Difficulty.ToString()));
        }
    }
}
