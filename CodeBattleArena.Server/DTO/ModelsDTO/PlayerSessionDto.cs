using CodeBattleArena.Server.Models;
using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Server.DTO.ModelsDTO
{
    public class PlayerSessionDto
    {
        public string IdPlayer { get; set; }
        public PlayerDto? Player { get; set; }
        public int IdSession { get; set; }
        public SessionDto? Session { get; set; }

        public string? CodeText { get; set; }

        [StringLength(50)]
        public string? Time { get; set; }
        public int? Memory { get; set; }
        public DateTime? FinishTask { get; set; }
        public bool IsCompleted { get; set; }
    }
}
