namespace CodeBattleArena.Server.Services.Judge0
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
