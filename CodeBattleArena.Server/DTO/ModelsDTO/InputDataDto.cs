using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Server.DTO.ModelsDTO
{
    public class InputDataDto
    {
        public int? IdInputData { get; set; }
        [Required(ErrorMessage = "Data is required.")]
        public string Data { get; set; }
    }
}
