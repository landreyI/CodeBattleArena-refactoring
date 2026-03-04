using CodeBattleArena.Application.Common.Contracts;
using CodeBattleArena.Application.Common.Helpers;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.AI.Models;
using CodeBattleArena.Application.Features.ProgrammingTasks.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Enums;
using CodeBattleArena.Domain.ProgrammingLanguages;
using CodeBattleArena.Domain.ProgrammingTasks;
using CodeBattleArena.Domain.ProgrammingTasks.Value_Objects;
using CodeBattleArena.Domain.TaskLanguages;
using Microsoft.Extensions.Logging;

namespace CodeBattleArena.Application.Features.AI.Services
{
    public class AIService : IAIService
    {
        private readonly IRepository<ProgrammingTask> _taskRepository;
        private readonly IRepository<ProgrammingLang> _langRepository;
        private readonly IAIGenerationGateway _aiGateway;
        private readonly IJudge0Client _judge0Client;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AIService> _logger;
        private const int MaxRetryAttempts = 1;

        public AIService(
            IRepository<ProgrammingTask> taskRepository,
            IRepository<ProgrammingLang> langRepository,
            IAIGenerationGateway aiGateway,
            IUnitOfWork unitOfWork,
            IJudge0Client judge0Client,
            ILogger<AIService> logger)
        {
            _taskRepository = taskRepository;
            _langRepository = langRepository;
            _aiGateway = aiGateway;
            _unitOfWork = unitOfWork;
            _judge0Client = judge0Client;
            _logger = logger;
        }

        public async Task<Result<ProgrammingTask>> GenerateTaskMetadataAndTestsAsync(GenerateAITaskMessage generateAITaskMessage, CancellationToken ct = default)
        {
            ProgrammingTask? task = null;
            if (generateAITaskMessage.ExistingId.HasValue)
            {
                var spec = new ProgrammingTaskForUpdateSpec(generateAITaskMessage.ExistingId.Value);
                task = await _taskRepository.GetBySpecAsync(spec, ct);
            }
            
            var gatewayRequest = new RequestGeneratingAITask
            {
                Prompt = generateAITaskMessage.Prompt,
                Difficulty = generateAITaskMessage.Difficulty,
                ExistingTask = task
            };

            var aiResult = await _aiGateway.GenerateTaskMetadataAndTestsAsync(gatewayRequest, ct);
            if (aiResult.IsFailure) return Result<ProgrammingTask>.Failure(aiResult.Error);

            var finalGeneratedDto = aiResult.Value;
            if (finalGeneratedDto == null) 
                return Result<ProgrammingTask>.Failure(new Error("AI.Empty", "Generation failed."));

            // Создаем или обновляем саму задачу
            if (task == null)
            {
                var createResult = ProgrammingTask.Create(
                    finalGeneratedDto.Name,
                    finalGeneratedDto.Description,
                    Enum.Parse<Difficulty>(generateAITaskMessage.Difficulty, true),
                    generateAITaskMessage.UserId,
                    true);
                if (createResult.IsFailure) 
                    return Result<ProgrammingTask>.Failure(createResult.Error);
                task = createResult.Value;

                var resultSyncTestCases = task.SyncTestCases(finalGeneratedDto.TestCases.Select(tc => new TestCaseInfo(tc.Input, tc.ExpectedOutput)));
                if (resultSyncTestCases.IsFailure)
                    return Result<ProgrammingTask>.Failure(resultSyncTestCases.Error);

                await _taskRepository.AddAsync(task, ct);
            }
            else
            {
                var resultUpdate = task.Update(finalGeneratedDto.Name, finalGeneratedDto.Description, Enum.Parse<Difficulty>(generateAITaskMessage.Difficulty, true), true);
                if (resultUpdate.IsFailure)
                    return Result<ProgrammingTask>.Failure(resultUpdate.Error);

                var resultSyncTestCases = task.SyncTestCases(finalGeneratedDto.TestCases.Select(tc => new TestCaseInfo(tc.Input, tc.ExpectedOutput)));
                if (resultSyncTestCases.IsFailure)
                    return Result<ProgrammingTask>.Failure(resultSyncTestCases.Error);
            }

            return Result<ProgrammingTask>.Success(task);
        }

        public async Task<Result<(ProgrammingTask, TaskLanguage)>> GenerateLanguageCodeAsync(GenerateAILanguageCodeMessage generateAILanguageCodeMessage, CancellationToken ct = default)
        {
            var spec = new ProgrammingTaskForUpdateSpec(generateAILanguageCodeMessage.TaskId);
            var task = await _taskRepository.GetBySpecAsync(spec, ct);

            if (task == null) 
                return Result<(ProgrammingTask, TaskLanguage)>.Failure(new Error("Task.NotFound", "Programming task not found."));

            var lang = await _langRepository.GetByIdAsync(generateAILanguageCodeMessage.LanguageId, true, ct);
            if (lang == null) 
                return Result<(ProgrammingTask, TaskLanguage)>.Failure(new Error("Language.NotFound", "Programming language not found."));

            var taskLanguage = task.TaskLanguages.FirstOrDefault(tl => tl.ProgrammingLangId == generateAILanguageCodeMessage.LanguageId);

            AiTaskLanguageDto? finalGeneratedDto = null;

            string? errorApi = default;
            int attempt = 0;
            while (attempt <= MaxRetryAttempts)
            {
                var aiResult = await _aiGateway.GenerateLanguageCodeAsync(generateAILanguageCodeMessage.Prompt, lang.Name, taskLanguage, errorApi, ct);
                if (aiResult.IsFailure) 
                    return Result<(ProgrammingTask, TaskLanguage)>.Failure(aiResult.Error);
                if (aiResult.Value == null) 
                    return Result<(ProgrammingTask, TaskLanguage)>.Failure(new Error("AI.Empty", "Generation failed."));

                finalGeneratedDto = aiResult.Value;

                errorApi = default;

                var tempTests = task.TestCases.Select(tc => (tc.Input, tc.ExpectedOutput));

                var submission = CodeBuilder.Build(aiResult.Value.SolutionCode, aiResult.Value.VerificationCode, tempTests);

                var checkResult = await _judge0Client.CheckAsync(
                    submission.source_code,
                    lang.ExternalId,
                    submission.stdin,
                    submission.expected_output,
                    ct);

                if (checkResult.CompileOutput != null)
                    errorApi = checkResult.CompileOutput;
                else break;

                attempt++;
            }

            if (string.IsNullOrWhiteSpace(errorApi) && finalGeneratedDto != null)
            {
                var resultUpdate = task.UpsertTaskLanguage(lang.Id, finalGeneratedDto.Preparation, finalGeneratedDto.VerificationCode, true);
                if (resultUpdate.IsFailure) 
                    return Result<(ProgrammingTask, TaskLanguage)>.Failure(resultUpdate.Error);
            }
            else
                return Result<(ProgrammingTask, TaskLanguage)>.Failure(new Error("AI.GenerationFailed", $"Code validation failed: {errorApi}"));

            await _unitOfWork.CommitAsync(ct);

            return Result<(ProgrammingTask, TaskLanguage)>.Success(new (task , task.TaskLanguages.First(tl => tl.ProgrammingLangId == lang.Id)));
        }

        /*public async Task<Result<ProgrammingTask>> GenerateAIProgrammingTaskAsync(ProcessAIGenerationProgrammingTaskCommand request, CancellationToken ct = default)
        {
            // 1. Ищем задачу
            ProgrammingTask? task = null;
            if (request.ExistingId.HasValue)
            {
                var spec = new ProgrammingTaskForUpdateSpec(request.ExistingId.Value);
                task = await _taskRepository.GetBySpecAsync(spec, ct);
            }

            // 2. Готовим словарь языков (ID -> Name)
            Dictionary<Guid, string> targetLangs = new();
            if (request.ProgrammingLangIds != null && request.ProgrammingLangIds.Any())
            {
                var langs = await _langRepository.GetListAsync(l => request.ProgrammingLangIds.Contains(l.Id), true, ct);
                targetLangs = langs.ToDictionary(l => l.Id, l => l.Name);
            }
            else
            {
                var allLangs = await _langRepository.GetListAsync(l => true, true, ct);
                targetLangs = allLangs.ToDictionary(l => l.Id, l => l.Name);
            }

            // Ошибки для ИИ (по языкам)
            Dictionary<Guid, string> errorsForAi = new();
            AiGeneratedTaskDto? finalGeneratedDto = null;

            int attempt = 0;
            while (attempt <= MaxRetryAttempts)
            {
                var gatewayRequest = new RequestGeneratingAITask
                {
                    Prompt = request.Prompt,
                    ProgrammingLanguages = targetLangs,
                    Difficulty = request.Difficulty,
                    ExistingTask = task
                };

                var aiResult = await _aiGateway.GenerateTaskAsync(gatewayRequest, errorsForAi, ct);
                if (aiResult.IsFailure) return Result<ProgrammingTask>.Failure(aiResult.Error);

                finalGeneratedDto = aiResult.Value;
                errorsForAi.Clear(); // Очищаем старые ошибки перед новой проверкой

                // 4. ПРОВЕРКА КАЖДОГО ЯЗЫКА ЧЕРЕЗ JUDGE0
                bool hasErrorsInThisAttempt = false;

                foreach (var langDto in finalGeneratedDto.TaskLanguages)
                {
                    // Нам нужен IdCheckApi для Judge0, берем его из БД через репозиторий
                    var langEntity = await _langRepository.GetByIdAsync(langDto.ProgrammingLangId, true, ct);
                    if (langEntity == null) continue;

                    // Собираем временные объекты для CodeBuilder.Build
                    var tempTests = finalGeneratedDto.TestCases.Select(tc => (tc.Input, tc.ExpectedOutput));

                    var submission = CodeBuilder.Build(langDto.SolutionCode, langDto.VerificationCode, tempTests);

                    var checkResult = await _judge0Client.CheckAsync(
                        submission.source_code,
                        langEntity.ExternalId,
                        submission.stdin,
                        submission.expected_output,
                        ct);

                    if (checkResult.CompileOutput != null)
                    {
                        errorsForAi[langDto.ProgrammingLangId] = checkResult.CompileOutput;
                        hasErrorsInThisAttempt = true;
                    }
                }

                if (!hasErrorsInThisAttempt) break; // Если ошибок нет - выходим из цикла попыток
                attempt++;
            }

            if (finalGeneratedDto == null)
                return Result<ProgrammingTask>.Failure(new Error("AI.Empty", "Generation failed."));

            // 5. ПРИМЕНЕНИЕ РЕЗУЛЬТАТОВ (Синхронизация с БД)
            // Создаем или обновляем саму задачу
            if (task == null)
            {
                var createResult = ProgrammingTask.Create(
                    finalGeneratedDto.Name,
                    finalGeneratedDto.Description,
                    Enum.Parse<Difficulty>(request.Difficulty, true),
                    request.UserId,
                    true);
                if (createResult.IsFailure) return Result<ProgrammingTask>.Failure(createResult.Error);
                task = createResult.Value;
                await _taskRepository.AddAsync(task, ct);
            }
            else
            {
                task.Update(finalGeneratedDto.Name, finalGeneratedDto.Description, Enum.Parse<Difficulty>(request.Difficulty, true), true);
            }


            // Синхронизируем тесты
            task.SyncTestCases(finalGeneratedDto.TestCases.Select(tc => (tc.Input, tc.ExpectedOutput)));

            // Синхронизируем языки (только те, что прошли проверку)
            var verifyTaskLanguages = finalGeneratedDto.TaskLanguages.Where(tk => !errorsForAi.ContainsKey(tk.ProgrammingLangId));
            task.SyncTaskLanguages(verifyTaskLanguages.Select(tk => (tk.ProgrammingLangId, tk.Preparation, tk.VerificationCode, true)));
            
            await _unitOfWork.CommitAsync(ct);
            return Result<ProgrammingTask>.Success(task);
        }*/
    }
}