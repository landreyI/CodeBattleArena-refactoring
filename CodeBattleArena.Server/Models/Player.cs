using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeBattleArena.Server.Models
{
    public class Player : IdentityUser
    {
        public string? PhotoUrl { get; set; }
        public string? AdditionalInformation { get; set; }
        public int Victories { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? CountGames { get; set; }
        public int? Coin { get; set; }
        public int? Experience { get; set; }

        [NotMapped]
        public List<string>? Roles { get; set; }

        public virtual ICollection<PlayerSession>? PlayerSessions { get; set; }
        public virtual ICollection<PlayerTaskPlay>? PlayerTaskPlays { get; set; }
        public virtual ICollection<PlayerItem>? PlayerItems { get; set; }
        public virtual ICollection<Friend>? Friends1 { get; set; }
        public virtual ICollection<Friend>? Friends2 { get; set; }
        public virtual ICollection<Chat>? ChatsAsUser1 { get; set; }
        public virtual ICollection<Chat>? ChatsAsUser2 { get; set; }
        public virtual ICollection<Message>? Messages { get; set; }
        public virtual ICollection<TaskProgramming>? TaskProgrammings { get; set; }

        public int? ActiveBackgroundId { get; set; }
        public Item? ActiveBackground { get; set; }

        public int? ActiveAvatarId { get; set; }
        public Item? ActiveAvatar { get; set; }

        public int? ActiveBadgeId { get; set; }
        public Item? ActiveBadge { get; set; }

        public int? ActiveBorderId { get; set; }
        public Item? ActiveBorder { get; set; }

        public int? ActiveTitleId { get; set; }
        public Item? ActiveTitle { get; set; }
    }
}
