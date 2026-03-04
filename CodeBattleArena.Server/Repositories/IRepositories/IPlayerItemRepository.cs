using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Specifications;

namespace CodeBattleArena.Server.Repositories.IRepositories
{
    public interface IPlayerItemRepository
    {
        Task<PlayerItem> GetPlayerItemAsync(ISpecification<PlayerItem> specification, CancellationToken cancellationToken);
        Task<List<PlayerItem>> GetListPlayerItemAsync(ISpecification<PlayerItem> specification, CancellationToken cancellationToken);
        Task AddPlayerItemAsync(PlayerItem playerItem, CancellationToken cancellationToken);
        Task AddPlayersItemsAsync(List<PlayerItem> playersItems, CancellationToken cancellationToken);
        Task DeletePlayerItemAsync(int idItem, string idPlayer, CancellationToken cancellationToken);
        Task DeletePlayerItems(int idItem, CancellationToken cancellationToken);
    }
}
