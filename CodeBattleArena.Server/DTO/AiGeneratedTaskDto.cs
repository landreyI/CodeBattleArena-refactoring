namespace CodeBattleArena.Server.DTO
{
    public class AiGeneratedTaskDto
    {
        public string Name { get; set; }
        public string TextTask { get; set; }
        public string Preparation { get; set; }
        public string VerificationCode { get; set; }
        public List<TestCaseDto> TestCases { get; set; }

        public string SolutionCode { get; set; }
    } 
    public class TestCaseDto
    {
        public string Input { get; set; }
        public string Answer { get; set; }
    }
}
