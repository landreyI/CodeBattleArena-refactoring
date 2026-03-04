using CodeBattleArena.Server.DTO.ModelsDTO;

namespace CodeBattleArena.Server.Helpers
{
    public class CreateTaskPlayRequest
    {
        public TaskPlayDto TaskPlay { get; set; }
        public List<int> IdRewards { get; set; }
    }
}
