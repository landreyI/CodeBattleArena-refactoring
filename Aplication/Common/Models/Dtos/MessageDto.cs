
namespace CodeBattleArena.Application.Common.Models.Dtos
{
    public class MessageDto
    {
        public Guid? IdSender { get; set; }
        public PlayerDto? Sender { get; set; }

        public string? MessageText { get; set; } = string.Empty;
    }
}
