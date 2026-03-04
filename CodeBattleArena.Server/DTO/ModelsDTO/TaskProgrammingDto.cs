using CodeBattleArena.Server.Enums;
using CodeBattleArena.Server.Models;
using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Server.DTO.ModelsDTO
{
    public class TaskProgrammingDto
    {
        public int? IdTaskProgramming { get; set; }
        [Required(ErrorMessage = "Session Name is required.")]
        [MinLength(2, ErrorMessage = "Name must be at least 2 characters long.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Lang programming is required.")]
        public int LangProgrammingId { get; set; }
        public LangProgrammingDto LangProgramming { get; set; }
        [Required(ErrorMessage = "Difficulty is required.")]
        public Difficulty Difficulty { get; set; }

        [Required(ErrorMessage = "TextTask is required.")]
        public string TextTask { get; set; }

        [Required(ErrorMessage = "Preparation is required.")]
        public string Preparation { get; set; }

        [Required(ErrorMessage = "VerificationCode is required.")]
        public string VerificationCode { get; set; }
        public bool IsGeneratedAI { get; set; }
        public string? IdPlayer { get; set; }
        public PlayerDto? Player { get; set; }

        public List<TaskInputDataDto>? TaskInputData { get; set; }
    }
}
