using CodeBattleArena.Server.Enums;

namespace CodeBattleArena.Server.DTO.ModelsDTO
{
    public class TaskPlayParamDto
    {
        public int? IdParam { get; set; }

        public int? TaskPlayId { get; set; }

        public TaskParamKey ParamKey { get; set; }
        public string ParamValue { get; set; }
        public bool IsPrimary { get; set; }
    }
}
