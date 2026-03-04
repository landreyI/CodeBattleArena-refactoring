using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Server.Models
{
    public class PlayerTaskPlay
    {
        [Key]
        public int IdPlayerTaskPlay { get; set; }

        public string PlayerId { get; set; }
        public virtual Player? Player { get; set; }

        public int TaskPlayId { get; set; }
        public virtual TaskPlay? TaskPlay { get; set; }

        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsGet { get; set; }
        public string? ProgressValue { get; set; }
    }
}
