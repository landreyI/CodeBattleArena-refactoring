using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Server.Models
{
    public class League
    {
        [Key]
        public int IdLeague { get; set; }
        [StringLength(30)]
        public string Name { get; set; }
        public string? PhotoUrl { get; set; }
        public int MinWins { get; set; }
        public int? MaxWins { get; set; }
    }
}
