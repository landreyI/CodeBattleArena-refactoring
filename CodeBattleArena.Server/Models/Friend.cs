using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Server.Models
{
    public class Friend
    {
        [Key]
        public int IdFriend { get; set; }
        public string RequesterId { get; set; }
        public virtual Player Requester { get; set; }

        public string AddresseeId { get; set; }
        public virtual Player Addressee { get; set; }

        public bool IsFriendship { get; set; }

        public DateTime? FriendshipDate { get; set; }
    }
}
