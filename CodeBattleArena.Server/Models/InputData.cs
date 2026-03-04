using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Server.Models
{
    public class InputData
    {
        [Key]
        public int IdInputData { get; set; }
        public string Data { get; set; }

        public virtual ICollection<TaskInputData>? TaskInputData { get; set; }
    }
}
