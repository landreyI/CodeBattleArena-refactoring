using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Server.Models
{
    public class PlayerSession
    {
        public string IdPlayer { get; set; }
        public virtual Player? Player { get; set; }
        public int IdSession { get; set; }
        public virtual Session? Session { get; set; }

        public string? CodeText { get; set; }

        [StringLength(50)]
        public string? Time { get; set; }
        public int? Memory { get; set; }
        public DateTime? FinishTask { get; set; }
        public bool IsCompleted { get; set; }
    }
}
