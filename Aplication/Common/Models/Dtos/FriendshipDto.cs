namespace CodeBattleArena.Application.Common.Models.Dtos
{
    public class FriendshipDto
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public PlayerDto? Sender { get; set; }

        public Guid ReceiverId { get; set; }
        public PlayerDto? Receiver { get; set; }

        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
    }
}
