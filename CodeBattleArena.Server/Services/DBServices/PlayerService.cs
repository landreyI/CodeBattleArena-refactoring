using AutoMapper;
using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Enums;
using CodeBattleArena.Server.Filters;
using CodeBattleArena.Server.Helpers;
using CodeBattleArena.Server.IRepositories;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Services.DBServices.IDBServices;
using CodeBattleArena.Server.Untils;
using Microsoft.AspNetCore.Identity;
using System.Threading;

namespace CodeBattleArena.Server.Services.DBServices
{
    public class PlayerService : IPlayerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PlayerService> _logger;
        private readonly UserManager<Player> _userManager;
        private readonly IMapper _mapper;
        public PlayerService(IUnitOfWork unitOfWork, ILogger<PlayerService> logger, 
            UserManager<Player> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<Result<(PlayerDto Player, bool IsEdit), ErrorResponse>> GetPlayerInfoAsync
            (string targetId, string requesterId, CancellationToken cancellationToken)
        {
            var player = await GetPlayerAsync(targetId, cancellationToken);
            if (player == null)
            {
                return Result.Failure<(PlayerDto, bool), ErrorResponse>(new ErrorResponse
                {
                    Error = "Player not found."
                });
            }

            var playerDto = _mapper.Map<PlayerDto>(player);

            bool isEdit = false;

            bool isAuth = requesterId == playerDto.Id;

            if (isAuth)
                isEdit = true;

            else if (!isAuth && !string.IsNullOrEmpty(requesterId))
            {
                var requesterRoles = await GetRolesAsync(requesterId);
                isEdit = BusinessRules.IsModerationRole(requesterRoles);
            }

            if (isEdit) 
            {
                var targetRoles = await GetRolesAsync(targetId);
                playerDto.Roles = targetRoles;
            }

            return Result.Success<(PlayerDto, bool), ErrorResponse>((playerDto, isEdit));
        }

        public async Task<Result<Unit, ErrorResponse>> ChangeActiveItem
            (string targetId, string requesterId, int idItem, CancellationToken ct, bool commit = true)
        {
            var item = await _unitOfWork.ItemRepository.GetItemAsync(idItem, ct);
            if (item == null)
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Item not found."
                });

            var user = await _userManager.FindByIdAsync(targetId);
            var dtoPlayer = _mapper.Map<PlayerDto>(user);

            dtoPlayer = BusinessRules.ChangeActiveItem(dtoPlayer, item);

            var resultUpdate = await UpdatePlayerAsync(requesterId, dtoPlayer, ct);
            if(!resultUpdate.IsSuccess)
                return Result.Failure<Unit, ErrorResponse>(resultUpdate.Failure);

            return Result.Success<Unit, ErrorResponse>(Unit.Value);
        }

        public async Task<Result<Unit, ErrorResponse>> UpdatePlayerAsync
            (string authUserId, PlayerDto dto, CancellationToken ct)
        {
            var user = await _userManager.FindByIdAsync(authUserId);
            if (user == null)
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Player not found."
                });

            var role = await GetRolesAsync(user);
            bool isEdit = Helpers.BusinessRules.IsModerationRole(role);

            if (authUserId != dto.Id && !isEdit)
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse 
                { 
                    Error = "You do not have the right to change other users." 
                });

            var IsUserNameTaken = await IsUserNameTakenAsync(dto.Username, dto.Id);
            if(!IsUserNameTaken)
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Username already taken."
                });

            _mapper.Map(dto, user);

            var resultUpdate = await UpdatePlayerInDbAsync(user);
            if (!resultUpdate.IsSuccess)
                return Result.Failure<Unit, ErrorResponse>(resultUpdate.Failure);

            return Result.Success<Unit, ErrorResponse> (Unit.Value);
        }

        public async Task<Result<Unit, ErrorResponse>> SelectRolesAsync(string idPlayer, string[] roles, CancellationToken ct)
        {
            var player = await GetPlayerAsync(idPlayer, ct);
            if (player == null)
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                { Error = "Player not found." });

            var currentRoles = await GetRolesAsync(idPlayer);

            if (roles.Contains(Role.Admin.ToString()) && !currentRoles.Contains(Role.Admin.ToString()))
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse 
                { Error = $"You cannot add the {Role.Admin} role." });

            var protectedRoles = new[] { Role.Admin.ToString() };

            var addRoles = roles.Except(currentRoles)
                .Where(r => !protectedRoles.Contains(r));
            var delRoles = currentRoles.Except(roles)
                .Where(r => !protectedRoles.Contains(r));

            var resultAdd = await _userManager.AddToRolesAsync(player, addRoles);
            if (!resultAdd.Succeeded)
            {
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Failed to add roles",
                    Details = resultAdd.Errors
                        .ToDictionary(
                            e => e.Code,
                            e => e.Description
                        )
                });
            }

            var resultDelete = await _userManager.RemoveFromRolesAsync(player, delRoles);
            if (!resultDelete.Succeeded)
            {
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Failed to delete roles",
                    Details = resultDelete.Errors
                        .ToDictionary(
                            e => e.Code,
                            e => e.Description
                        )
                });
            }

            return Result.Success<Unit, ErrorResponse>(Unit.Value);
        }

        public async Task<bool> IsUserNameTakenAsync(string username, string id)
        {
            var existingUser = await _userManager.FindByNameAsync(username);
            if (existingUser != null && existingUser.Id != id)
                return false;

            return true;
        }
        public async Task<IList<string>> GetRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);
            return roles;
        }
        public async Task<IList<string>> GetRolesAsync(Player user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles;
        }



        public async Task<Player> GetPlayerAsync(string id, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(id)) return null;
            return await _unitOfWork.PlayerRepository.GetPlayerAsync(id, ct);
        }
        public async Task<List<Player>> GetPlayersAsync(IFilter<Player>? filter, CancellationToken ct)
        {
            return await _unitOfWork.PlayerRepository.GetPlayersAsync(filter, ct);
        }
        public async Task<Result<Unit, ErrorResponse>> AddVictoryPlayerInDbAsync(string id, CancellationToken ct, bool commit = true)
        {
            try
            {
                await _unitOfWork.PlayerRepository.AddVictoryPlayerAsync(id, ct);
                if (commit)
                    await _unitOfWork.CommitAsync(ct);

                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding AddVictoryPlayer");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse { Error = "Database error when update player." });
            }
        }
        public async Task<List<Session>> MyGamesListAsync(string id, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(id)) return null;
            return await _unitOfWork.PlayerRepository.MyGamesListAsync(id, ct);
        }
        public async Task<Result<Unit, ErrorResponse>> UpdatePlayerInDbAsync(Player player)
        {
            var result = await _userManager.UpdateAsync(player);
            if (!result.Succeeded)
            {
                var error = new ErrorResponse
                {
                    Error = "Error saving edited data.",
                    Details = result.Errors
                        .Select((e, i) => new KeyValuePair<string, string>($"db_{i}", e.Description))
                        .ToDictionary(x => x.Key, x => x.Value)
                };
                return Result.Failure<Unit, ErrorResponse>(error);
            }

            return Result.Success<Unit, ErrorResponse>(Unit.Value);
        }
    }
}
