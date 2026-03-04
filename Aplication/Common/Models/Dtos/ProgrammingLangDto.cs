namespace CodeBattleArena.Application.Common.Models.Dtos
{
    public class ProgrammingLangDto
    {
        public Guid Id { get; set; }
        public string Alias { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ExternalId { get; set; } = string.Empty;
    }
}
