using AutoMapper;
using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Infrastructure.Attributes;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Services.DBServices.IDBServices;
using CodeBattleArena.Server.Services.Notifications.INotifications;
using CodeBattleArena.Server.Untils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CodeBattleArena.Server.Controllers
{
    [ApiController]
    [Authorize]
    [RequireEditRole]
    [Route("api/[controller]")]
    public class AdminTaskController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly ITaskNotificationService _taskNotificationService;
        private readonly ILangProgrammingService _langProgrammingService;
        private readonly IMapper _mapper;
        private readonly UserManager<Player> _userManager;
        public AdminTaskController(ITaskService taskService, ITaskNotificationService taskNotificationService, 
            IMapper mapper, ILangProgrammingService langProgrammingService, UserManager<Player> userManager)
        {
            _taskService = taskService;
            _taskNotificationService = taskNotificationService;
            _mapper = mapper;
            _langProgrammingService = langProgrammingService;
            _userManager = userManager;
        }

        [HttpDelete("delete-task-programming")]
        public async Task<IActionResult> DeleteTask(int? id, CancellationToken cancellationToken)
        {
            if (id == null) return BadRequest(new ErrorResponse { Error = "Task ID not specified." });

            var result = await _taskService.DeleteTaskProgrammingInDbAsync(id.Value, cancellationToken);
            if (!result.IsSuccess)
                return UnprocessableEntity(result.Failure);

            await _taskNotificationService.NotifyTaskDeletedGroupAsync(id.Value);
            await _taskNotificationService.NotifyTaskDeletedAllAsync(id.Value);

            return Ok(true);
        }

        [HttpPost("create-task-programming")]
        public async Task<IActionResult> CreateTask([FromBody] TaskProgrammingDto taskProgrammingDto, CancellationToken cancellationToken)
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
            var createResult = await _taskService.CreateTaskProgrammingAsync(taskProgrammingDto, currentUserId, cancellationToken);
            if (!createResult.IsSuccess)
                return UnprocessableEntity(createResult.Failure);

            var task = createResult.Success;
            var dtoTask = _mapper.Map<TaskProgrammingDto>(task);
            var lang = await _langProgrammingService.GetLangProgrammingAsync(task.LangProgrammingId, cancellationToken);
            var dtoLang = _mapper.Map<LangProgrammingDto>(lang);

            dtoTask.LangProgramming = dtoLang;

            await _taskNotificationService.NotifyTaskAddAsync(dtoTask);

            return Ok(task.IdTaskProgramming);
        }

        [HttpPut("edit-task-programming")]
        public async Task<IActionResult> EditTask([FromBody] TaskProgrammingDto taskProgrammingDto, CancellationToken cancellationToken)
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

            var resultUpdate = await _taskService.UpdateTaskProgrammingAsync(taskProgrammingDto, cancellationToken);
            if (!resultUpdate.IsSuccess)
                return UnprocessableEntity(resultUpdate.Failure);

            await _taskNotificationService.NotifyTaskUpdatedGroupAsync(taskProgrammingDto);
            await _taskNotificationService.NotifyTaskUpdatedAllAsync(taskProgrammingDto);

            return Ok(true);
        }
    }
}
