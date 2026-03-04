using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Server.Models
{
    public class Chat
    {
        [Key]
        public int IdChat { get; set; }

        public string IdPlayer1 { get; set; }
        public virtual Player? Player1 { get; set; }
        public string IdPlayer2 { get; set; }
        public virtual Player? Player2 { get; set; }

        public virtual ICollection<Message>? Messages { get; set; }
    }

}
