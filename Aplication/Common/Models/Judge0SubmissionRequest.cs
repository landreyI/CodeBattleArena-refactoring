namespace CodeBattleArena.Application.Common.Models
{
    public class Judge0SubmissionRequest
    {
        public string source_code { get; set; } = string.Empty;
        public string language_id { get; set; } = string.Empty;
        public string stdin { get; set; } = string.Empty;
        public string expected_output { get; set; } = string.Empty;
    }
}
