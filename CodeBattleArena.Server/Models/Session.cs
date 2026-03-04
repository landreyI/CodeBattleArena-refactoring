using CodeBattleArena.Server.Enums;
using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Server.Models
{
    public class Session
    {
        [Key]
        public int IdSession { get; set; }
        [StringLength(20)]
        public string Name { get; set; }

        public int LangProgrammingId { get; set; }
        public LangProgramming? LangProgramming { get; set; }


        [StringLength(20)]
        public SessionState State { get; set; }
        public int MaxPeople { get; set; }

        public int? TimePlay { get; set; } //Minutes

        public int? TaskId { get; set; }
        public virtual TaskProgramming? TaskProgramming { get; set; }

        public string? WinnerId { get; set; }
        public string CreatorId { get; set; }

        public string? Password { get; set; }
        public DateTime DateCreating { get; set; }
        public DateTime? DateStartGame { get; set; }
        public bool IsStart { get; set; }
        public bool IsFinish { get; set; }

        public virtual ICollection<PlayerSession>? PlayerSessions { get; set; }
    }
}
