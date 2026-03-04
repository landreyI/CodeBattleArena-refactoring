using CodeBattleArena.Server.Filters;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Specifications;
using CodeBattleArena.Server.Untils;

namespace CodeBattleArena.Server.Services.DBServices.IDBServices
{
    public interface IItemService
    {
        Task<Item> GetItemAsync(int id, CancellationToken cancellationToken);
        Task<List<Item>> GetItemsAsync(IFilter<Item>? filter, CancellationToken cancellationToken);
        Task<PlayerItem> GetPlayerItemAsync(int idItem, string idPlayer, CancellationToken cancellationToken);
        Task<List<PlayerItem>> GetListPlayerItemByIdItemAsync(ISpecification<PlayerItem> spec, CancellationToken cancellationToken);
        Task<Result<Unit, ErrorResponse>> BuyItemAsync(string idAuthPlayer, PlayerItem playerItem, CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> DeleteItemAsync(int id, CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> AddItemAsync(Item item, CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> DeleteItemInDbAsync(int id, CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> UpdateItem(Item item, CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> AddPlayerItemAsync(PlayerItem playerItem, CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> AddPlayersItemsAsync(List<PlayerItem> playersItems, CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> DeletePlayerItemAsync(int idItem, string idPlayer, CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> DeletePlayerItemsAsync(int idItem, CancellationToken cancellationToken, bool commit = true);
    }
}
