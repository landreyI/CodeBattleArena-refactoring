namespace CodeBattleArena.Application.Common.Models.Dtos
{
    public class SessionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Внешний ключ и полный объект
        public Guid ProgrammingLangId { get; set; }
        public ProgrammingLangDto? ProgrammingLang { get; set; }

        public string State { get; set; } = string.Empty;
        public int MaxPeople { get; set; }
        public int? TimePlay { get; set; }
        public string Status { get; set; } = string.Empty;

        // Задача
        public Guid? TaskId { get; set; }
        public ProgrammingTaskDto? ProgrammingTask { get; set; }

        public Guid? WinnerId { get; set; }
        public Guid CreatorId { get; set; }

        public DateTime DateCreating { get; set; }
        public DateTime? DateStartGame { get; set; }

        // Вычисляемые поля для удобства фронта
        public int AmountPeople { get; set; }
    }
}
