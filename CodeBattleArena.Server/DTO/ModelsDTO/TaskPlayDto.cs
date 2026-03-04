using CodeBattleArena.Server.Enums;

namespace CodeBattleArena.Server.DTO.ModelsDTO
{
    public class TaskPlayDto
    {
        public int? IdTask { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TaskType Type { get; set; }
        public int? Experience { get; set; }
        public int? RewardCoin { get; set; }
        public bool IsRepeatable { get; set; }
        public int? RepeatAfterDays { get; set; }
        public List<TaskPlayParamDto>? TaskPlayParams { get; set; }
    }
}
