using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Server.DTO.ModelsDTO
{
    public class ChatDto
    {
        public int? IdChat { get; set; }

        public string IdPlayer1 { get; set; }
        public PlayerDto? Player1 { get; set; }
        public string IdPlayer2 { get; set; }
        public PlayerDto? Player2 { get; set; }
    }

}
