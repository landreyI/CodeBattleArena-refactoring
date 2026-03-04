using AutoMapper;
using CodeBattleArena.Server.DTO;
using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Helpers;
using CodeBattleArena.Server.IRepositories;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Services.DBServices.IDBServices;
using CodeBattleArena.Server.Services.Gateways.IGateways;
using CodeBattleArena.Server.Services.Judge0;
using CodeBattleArena.Server.Untils;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace CodeBattleArena.Server.Services.DBServices
{
    public class AIService : IAIService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AIService> _logger;
        private readonly IMapper _mapper;
        private readonly ITaskService _taskService;
        private readonly IAIGenerationGateway _aiGateway;
        private readonly Judge0Client _judge0Client;
        private const int MaxRetryAttempts = 1;

        public AIService(IUnitOfWork unitOfWork, ILogger<AIService> logger, IMapper mapper, 
            IAIGenerationGateway aIGenerationGateway, ITaskService taskService, Judge0Client judge0Client)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _aiGateway = aIGenerationGateway;
            _taskService = taskService;
            _judge0Client = judge0Client;
        }

        public async Task<Result<TaskProgramming, ErrorResponse>> GenerateAITaskProgrammingAsync
            (RequestGeneratingAITaskDto dto, string userId, CancellationToken ct, bool commit = true)
        {
            var checkResult = ValidationHelper.CheckUserId<bool>(userId);
            if (!checkResult.IsSuccess)
                return Result.Failure<TaskProgramming, ErrorResponse>(checkResult.Failure);

            try
            {
                TaskProgramming? taskProgramming = null;

                if (dto.IdTaskProgramming != null)
                {
                    taskProgramming = await _unitOfWork.TaskRepository
                        .GetTaskProgrammingAsync(dto.IdTaskProgramming.Value, ct);
                }

                string? error = null;
                int attempts = 0;

                while (attempts <= MaxRetryAttempts)
                {
                    // 1. ВЫЗВАТЬ GATEWAY
                    var generatedDto = await _aiGateway.GenerateTaskAsync(dto, taskProgramming, error, ct);
                    if (generatedDto == null)
                    {
                        return Result.Failure<TaskProgramming, ErrorResponse>(new ErrorResponse
                        {
                            Error = "Failed to generate task from AI."
                        });
                    }

                    TaskProgramming newTask;
                    if (taskProgramming != null)
                    {
                        // Обновляем существующую задачу
                        taskProgramming.Name = generatedDto.Name;
                        taskProgramming.TextTask = generatedDto.TextTask;
                        taskProgramming.Preparation = generatedDto.Preparation;
                        taskProgramming.VerificationCode = generatedDto.VerificationCode;
                        taskProgramming.Difficulty = dto.Difficulty;
                        taskProgramming.LangProgrammingId = dto.LangProgrammingId;
                        taskProgramming.IsGeneratedAI = true;

                        // Очистить старые тест-кейсы и добавить новые
                        if(generatedDto.TestCases != null && generatedDto.TestCases.Count != 0)
                            taskProgramming?.TaskInputData?.Clear();

                        newTask = taskProgramming;
                    }
                    else
                    {
                        // Создаем новую задачу
                        newTask = new TaskProgramming
                        {
                            Name = generatedDto.Name,
                            TextTask = generatedDto.TextTask,
                            Preparation = generatedDto.Preparation,
                            VerificationCode = generatedDto.VerificationCode,
                            Difficulty = dto.Difficulty,
                            LangProgrammingId = dto.LangProgrammingId,
                            IsGeneratedAI = true
                        };
                    }

                    if (newTask.TaskInputData == null)
                        newTask.TaskInputData = new List<TaskInputData>();

                    // 2. Создать тест-кейсы
                    foreach (var testCase in generatedDto.TestCases)
                    {
                        var inputData = new InputData { Data = testCase.Input };
                        var taskInput = new TaskInputData
                        {
                            Answer = testCase.Answer,
                            InputData = inputData
                        };
                        newTask.TaskInputData.Add(taskInput);
                    }

                    // 3. Проверка решения через Judge0
                    var payload = CodeCheckBuilder.Build(generatedDto.SolutionCode, newTask, newTask.TaskInputData.ToList());

                    var result = await _judge0Client.CheckAsync(
                        payload.source_code,
                        dto.LangProgramming.IdCheckApi,
                        payload.stdin,
                        payload.expected_output
                    );

                    if (result.CompileOutput != null)
                    {
                        error = result.CompileOutput;
                        attempts++;
                        continue;
                    }

                    // 4. Устанавливаем пользователя
                    newTask.IdPlayer = userId;

                    // 5. Добавляем или обновляем в базе
                    if(taskProgramming == null)
                    {
                        var addResult = await _taskService.AddTaskProgrammingInDbAsync(newTask, ct, commit);
                        if (!addResult.IsSuccess)
                            return Result.Failure<TaskProgramming, ErrorResponse>(addResult.Failure);
                    }
                    else
                    {
                        if(taskProgramming.LangProgramming.IdLang != dto.LangProgrammingId)
                        {
                            newTask.LangProgramming = await _unitOfWork.LangProgrammingRepository
                                .GetLangProgrammingAsync(dto.LangProgrammingId, ct);
                        }
                        var updateResult = await _taskService.UpdateTaskProgrammingInDbAsync(newTask, ct, commit);
                        if (!updateResult.IsSuccess)
                            return Result.Failure<TaskProgramming, ErrorResponse>(updateResult.Failure);
                    }

                        _logger.LogInformation("AI task generated or updated: {TaskId}", newTask.IdTaskProgramming);
                    return Result.Success<TaskProgramming, ErrorResponse>(newTask);
                }

                return Result.Failure<TaskProgramming, ErrorResponse>(new ErrorResponse
                {
                    Error = "The number of attempts to generate AI has been exceeded. \nError:" + error
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GenerateAITaskProgrammingAsync");
                return Result.Failure<TaskProgramming, ErrorResponse>(new ErrorResponse
                {
                    Error = "An error occurred while generating the task."
                });
            }
        }
    }
}
