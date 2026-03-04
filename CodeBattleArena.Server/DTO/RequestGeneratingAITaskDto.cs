using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Enums;
using CodeBattleArena.Server.Models;
using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Server.DTO
{
    public class RequestGeneratingAITaskDto
    {
        public int? IdTaskProgramming { get; set; }

        [Required(ErrorMessage = "Lang programming is required.")]
        public int LangProgrammingId { get; set; }
        public LangProgrammingDto LangProgramming { get; set; }
        [Required(ErrorMessage = "Difficulty is required.")]
        public Difficulty Difficulty { get; set; }

        [Required(ErrorMessage = "Promt is required.")]
        public string Promt { get; set; }
    }
}
