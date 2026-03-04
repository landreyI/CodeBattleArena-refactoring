using CodeBattleArena.Server.Models;
using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Server.DTO.ModelsDTO
{
    public class TaskInputDataDto
    {
        public int? IdTaskProgramming { get; set; }
        public string Answer { get; set; }

        public int? IdInputDataTask { get; set; }
        public InputDataDto? InputData { get; set; }
    }
}
