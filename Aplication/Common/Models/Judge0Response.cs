
namespace CodeBattleArena.Application.Common.Models
{
    public class Judge0Response
    {
        public string? Stdout { get; set; }
        public string? Time { get; set; }
        public int? Memory { get; set; }
        public string? Stderr { get; set; }
        public string? Compile_output { get; set; }
        public string? Message { get; set; }
        public Judge0Status Status { get; set; } = null!;
    }
}
