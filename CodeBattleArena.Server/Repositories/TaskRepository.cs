using CodeBattleArena.Server.Data;
using CodeBattleArena.Server.Filters;
using CodeBattleArena.Server.IRepositories;
using CodeBattleArena.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeBattleArena.Server.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDBContext _context;

        public TaskRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task AddTaskProgrammingAsync(TaskProgramming taskProgramming, CancellationToken cancellationToken)
        {
            await _context.TasksProgramming.AddAsync(taskProgramming, cancellationToken);
        }
        public async Task AddInputDataAsync(string data, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(data)) return;
            var inputDataModel = new InputData { Data = data };
            await _context.InputData.AddAsync(inputDataModel);
        }
        public async Task AddTaskInputDataAsync(TaskInputData taskInputData, CancellationToken cancellationToken)
        {
            await _context.TaskInputData.AddAsync(taskInputData);
        }
        public async Task<List<InputData>> GetInputDataListAsync(CancellationToken cancellationToken)
        {
            return await _context.InputData.ToListAsync(cancellationToken);
        }
        public async Task<List<TaskInputData>> GetTaskInputDataByIdTaskProgrammingAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.TaskInputData
                .Where(t => t.IdTaskProgramming == id)
                .Include(t => t.TaskProgramming)
                .Include(i => i.InputData)
                .ToListAsync(cancellationToken);
        }
        public async Task<List<TaskProgramming>> GetTaskProgrammingListAsync(IFilter<TaskProgramming>? filter, CancellationToken cancellationToken)
        {
            var query = _context.TasksProgramming.AsQueryable();

            if (filter != null)
                query = filter.ApplyTo(query);

            return await query
                .Include(p => p.Player)
                .Include(s => s.LangProgramming)
                .ToListAsync(cancellationToken);
        }
        public async Task<TaskProgramming> GetTaskProgrammingAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.TasksProgramming
                .Include(l => l.LangProgramming)
                .Include(p => p.Player)
                .Include(t => t.TaskInputData)
                .ThenInclude(i => i.InputData)
                .FirstOrDefaultAsync(t => t.IdTaskProgramming == id, cancellationToken);
        }
        public async Task<InputData> GetInputDataById(int id)
        {
            return await _context.InputData.FirstOrDefaultAsync(i => i.IdInputData == id);
        }
        public Task UpdateTaskProgrammingAsync(TaskProgramming taskProgramming)
        {
            _context.TasksProgramming.Update(taskProgramming);
            return Task.CompletedTask;
        }
        public Task UpdateTaskInputDataAsync(TaskInputData taskInputData)
        {
            _context.TaskInputData.Update(taskInputData);
            return Task.CompletedTask;
        }
        public Task UpdateInputDataAsync(InputData inputData)
        {
            _context.InputData.Update(inputData);
            return Task.CompletedTask;
        }
        public async Task DeleteTaskInputDataAsync(int idTaskProgramming, int idInputData, CancellationToken cancellationToken)
        {
            var taskInputData = await _context.TaskInputData
                .FirstOrDefaultAsync(t => t.IdTaskProgramming == idTaskProgramming && t.IdInputDataTask == idInputData, 
                                    cancellationToken);
            if (taskInputData != null) _context.TaskInputData.Remove(taskInputData);
        }
        public async Task DeleteTaskProgrammingAsync(int id, CancellationToken cancellationToken)
        {
            var taskProgramming = await _context.TasksProgramming
                .FirstOrDefaultAsync(t => t.IdTaskProgramming == id, cancellationToken);
            if(taskProgramming != null) _context.TasksProgramming.Remove(taskProgramming);
        }
        public Task DeleteListTaskInputDatas(List<TaskInputData> taskInputDatas)
        {
            _context.TaskInputData.RemoveRange(taskInputDatas);
            return Task.CompletedTask;
        }
    }
}
