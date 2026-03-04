namespace CodeBattleArena.Application.Common.Models.Dtos
{
    public class PlayerSessionDto
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public PlayerDto? Player { get; set; }

        public Guid SessionId { get; set; }
        public SessionDto? Session { get; set; }

        public string? CodeText { get; set; }
        public string? Time { get; set; }
        public int? Memory { get; set; }
        public DateTime? FinishTask { get; set; }

        public bool IsCompleted { get; set; }
    }
}
