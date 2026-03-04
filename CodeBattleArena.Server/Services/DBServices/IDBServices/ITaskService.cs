using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Filters;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Untils;


namespace CodeBattleArena.Server.Services.DBServices.IDBServices
{
    public interface ITaskService
    {
        Task<Result<TaskProgramming, ErrorResponse>> CreateTaskProgrammingAsync(TaskProgrammingDto dto, string palyerId, CancellationToken ct, bool commit = true);
        Task<Result<Unit, ErrorResponse>> UpdateTaskProgrammingAsync(TaskProgrammingDto dto, CancellationToken ct, bool commit = true);
        Task<Result<Unit, ErrorResponse>> AddTaskProgrammingInDbAsync(TaskProgramming taskProgramming, CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> AddInputDataInDbAsync(string data, CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> AddTaskInputDataInDbAsync(TaskInputData taskInputData, CancellationToken cancellationToken, bool commit = true);
        Task<List<InputData>> GetInputDataListAsync(CancellationToken cancellationToken);
        Task<List<TaskInputData>> GetTaskInputDataByIdTaskProgrammingAsync(int id, CancellationToken cancellationToken);
        Task<List<TaskProgramming>> GetTaskProgrammingListAsync(IFilter<TaskProgramming>? filter, CancellationToken cancellationToken);
        Task<TaskProgramming> GetTaskProgrammingAsync(int id, CancellationToken cancellationToken);
        Task<Result<Unit, ErrorResponse>> UpdateTaskProgrammingInDbAsync(TaskProgramming taskProgramming, CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> DeleteTaskInputDataInDbAsync(int idTaskProgramming, int idInputData, CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> DeleteTaskProgrammingInDbAsync(int id, CancellationToken cancellationToken, bool commit = true);
    }
}
