namespace CodeBattleArena.Server.Models
{
    public class PlayerItem
    {
        public string IdPlayer { get; set; }
        public virtual Player? Player { get; set; }

        public int IdItem { get; set; }
        public virtual Item? Item { get; set; }
    }
}
