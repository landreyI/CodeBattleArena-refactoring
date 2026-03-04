using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Server.DTO.ModelsDTO
{
    public class MessageDto
    {
        public int? IdMessage { get; set; }

        public int? IdChat { get; set; }
        public ChatDto? Chat { get; set; }
        public string? IdSender { get; set; }
        public PlayerDto? Sender { get; set; }

        public string MessageText { get; set; }

    }
}
