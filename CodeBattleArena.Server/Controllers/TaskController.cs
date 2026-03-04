using AutoMapper;
using CodeBattleArena.Server.DTO;
using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Enums;
using CodeBattleArena.Server.Filters;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Services.DBServices;
using CodeBattleArena.Server.Services.DBServices.IDBServices;
using CodeBattleArena.Server.Services.Notifications.INotifications;
using CodeBattleArena.Server.Specifications.ItemSpec;
using CodeBattleArena.Server.Untils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CodeBattleArena.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly IAIService _aiService;
        private readonly ILangProgrammingService _langProgrammingService;
        private readonly ITaskNotificationService _taskNotificationService;
        private readonly IMapper _mapper;
        private readonly UserManager<Player> _userManager;
        public TaskController(ITaskService taskService, IMapper mapper, IAIService aiService, 
            ILangProgrammingService langProgrammingService, ITaskNotificationService taskNotificationService,
            UserManager<Player> userManager)
        {
            _taskService = taskService;
            _mapper = mapper;
            _aiService = aiService;
            _langProgrammingService = langProgrammingService;
            _taskNotificationService = taskNotificationService;
            _userManager = userManager;
        }

        [HttpGet("info-task-programming")]
        public async Task<IActionResult> GetTask(int? id, CancellationToken cancellationToken)
        {
            if (id == null) return BadRequest(new ErrorResponse { Error = "Task ID not specified." });

            var task = await _taskService.GetTaskProgrammingAsync(id.Value, cancellationToken);
            if (task == null) return NotFound(new ErrorResponse { Error = "Task not found." });

            var taskDto = _mapper.Map<TaskProgrammingDto>(task);

            return Ok(taskDto);
        }

        [HttpGet("list-tasks-programming")]
        public async Task<IActionResult> GetTasks([FromQuery] TaskProgrammingFilter? filter, CancellationToken cancellationToken)
        {
            var listTasks =  await _taskService.GetTaskProgrammingListAsync(filter, cancellationToken);
            var taskDtos = _mapper.Map<List<TaskProgrammingDto>>(listTasks);
            return Ok(taskDtos);
        }

        [HttpGet("list-input-datas")]
        public async Task<IActionResult> GetInputDatas(CancellationToken cancellationToken)
        {
            var listData = await _taskService.GetInputDataListAsync(cancellationToken);

            var dtos = _mapper.Map<List<InputDataDto>>(listData);

            return Ok(dtos);
        }

        [HttpGet("list-player-tasks")]
        public async Task<IActionResult> GetPlayerTasks(string? idPlayer, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(idPlayer)) return BadRequest(new ErrorResponse { Error = "Player ID not specified." });

            var listPlayerTasks = await _taskService.GetTaskProgrammingListAsync
                (new TaskProgrammingFilter { IdPlayer = idPlayer }, cancellationToken);

            return Ok(_mapper.Map<List<TaskProgrammingDto>>(listPlayerTasks));
        }

        [Authorize]
        [HttpPost("generate-ai-task-programming")]
        public async Task<IActionResult> GenerateAITaskProgramming([FromBody] RequestGeneratingAITaskDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return UnprocessableEntity(errors);
            }

            var currentUserId = _userManager.GetUserId(User);
            var createResult = await _aiService.GenerateAITaskProgrammingAsync(dto, currentUserId, cancellationToken);
            if (!createResult.IsSuccess)
                return UnprocessableEntity(createResult.Failure);

            var task = createResult.Success;
            var dtoTask = _mapper.Map<TaskProgrammingDto>(task);
            var lang = await _langProgrammingService.GetLangProgrammingAsync(task.LangProgrammingId, cancellationToken);
            var dtoLang = _mapper.Map<LangProgrammingDto>(lang);

            dtoTask.LangProgramming = dtoLang;

            await _taskNotificationService.NotifyTaskAddAsync(dtoTask);

            return Ok(dtoTask);
        }
    }
}
