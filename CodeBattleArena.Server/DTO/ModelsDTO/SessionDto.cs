using CodeBattleArena.Server.Enums;
using CodeBattleArena.Server.Models;
using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Server.DTO.ModelsDTO
{
    public class SessionDto
    {
        public int? IdSession { get; set; }
        [Required(ErrorMessage = "Session Name is required.")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Lang programming is required.")]
        public int LangProgrammingId { get; set; }
        public LangProgrammingDto? LangProgramming { get; set; }

        [Required(ErrorMessage = "State is required.")]
        public SessionState State { get; set; }

        [Range(1, 10, ErrorMessage = "MaxPeople must be between 1 and 10.")]
        public int MaxPeople { get; set; }
        public int? TimePlay { get; set; } //Minutes
        public int? TaskId { get; set; }
        public TaskProgrammingDto? TaskProgramming { get; set; }
        public string? WinnerId { get; set; }
        public string CreatorId { get; set; }
        public string? Password { get; set; }

        [Required(ErrorMessage = "DateCreating is required.")]
        public DateTime DateCreating { get; set; }
        public DateTime? DateStartGame { get; set; }
        public bool IsStart { get; set; }
        public bool IsFinish { get; set; }
        public int? AmountPeople { get; set; }
    }
}
