namespace CodeBattleArena.Server.DTO.ModelsDTO
{
    public class PlayerTaskPlayDto
    {
        public int? IdPlayerTaskPlay { get; set; }

        public string PlayerId { get; set; }
        public PlayerDto? Player { get; set; }

        public int TaskPlayId { get; set; }
        public TaskPlayDto? TaskPlay { get; set; }

        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsGet { get; set; }
        public string? ProgressValue { get; set; }
    }
}
