using CodeBattleArena.Server.Models;
using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Server.DTO.ModelsDTO
{
    public class PlayerDto
    {
        [Required(ErrorMessage = "Player ID is required.")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Player Username is required.")]
        [MinLength(2, ErrorMessage = "Username must be at least 2 characters long.")]
        public string Username { get; set; }
        public string? PhotoUrl { get; set; }
        public int? Coin { get; set; }
        public int? Experience { get; set; }
        public IList<string?>? Roles { get; set; }
        public int Victories { get; set; }
        public string? AdditionalInformation { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? CountGames { get; set; }

        public int? ActiveBackgroundId { get; set; }
        public ItemDto? ActiveBackground { get; set; }

        public int? ActiveAvatarId { get; set; }
        public ItemDto? ActiveAvatar { get; set; }

        public int? ActiveBadgeId { get; set; }
        public ItemDto? ActiveBadge { get; set; }

        public int? ActiveBorderId { get; set; }
        public ItemDto? ActiveBorder { get; set; }

        public int? ActiveTitleId { get; set; }
        public ItemDto? ActiveTitle { get; set; }
    }
}
