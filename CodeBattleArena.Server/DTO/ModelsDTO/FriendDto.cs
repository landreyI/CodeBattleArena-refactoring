using CodeBattleArena.Server.Models;

namespace CodeBattleArena.Server.DTO.ModelsDTO
{
    public class FriendDto
    {
        public int? IdFriend { get; set; }
        public string RequesterId { get; set; }
        public PlayerDto? Requester { get; set; }

        public string AddresseeId { get; set; }
        public PlayerDto? Addressee { get; set; }

        public bool IsFriendship { get; set; }

        public DateTime? FriendshipDate { get; set; }
    }
}
