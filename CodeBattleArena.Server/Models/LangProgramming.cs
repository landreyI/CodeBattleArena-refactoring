using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Server.Models
{
    public class LangProgramming
    {
        [Key]
        public int IdLang { get; set; }

        [StringLength(30)]
        public string CodeNameLang { get; set; }

        [StringLength(30)]
        public string NameLang { get; set; }
        public string IdCheckApi { get; set; }
        public virtual ICollection<Session>? Sessions { get; set; }
        public virtual ICollection<TaskProgramming>? TasksProgramming { get; set; }
    }
}
