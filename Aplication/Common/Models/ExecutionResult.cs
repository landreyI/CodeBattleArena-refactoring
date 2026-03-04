namespace CodeBattleArena.Application.Common.Models
{
    public class ExecutionResult
    {
        public string? Status { get; set; }
        public string[]? Stdout { get; set; }
        public string? Time { get; set; }
        public int? Memory { get; set; }
        public string? CompileOutput { get; set; }
    }
}
