namespace CodeBattleArena.Server.Untils
{
    public class ErrorResponse
    {
        public string Error { get; set; }
        public string? Code { get; set; }
        public Dictionary<string, string>? Details { get; set; }
    }
}
