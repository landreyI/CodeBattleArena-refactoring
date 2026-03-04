using CodeBattleArena.Server.Models;

namespace CodeBattleArena.Server.IRepositories
{
    public interface IChatRepository
    {
        Task AddChatAsync(string playerId1, string playerId2, CancellationToken cancellationToken);
        Task AddMessageAsync(int chatId, string senderId, string messageText, CancellationToken cancellationToken);
        Task<List<Message>> GetMessagesByChatIdAsync(int chatId, CancellationToken cancellationToken);
        Task<Chat> GetChatAsync(int idChat, CancellationToken cancellationToken);
        Task<Chat> GetChatByIdPlayerAsync(string playerId1, string playerId2, CancellationToken cancellationToken);
        Task<List<Chat>> GetChatsByPlayerIdAsync(string playerId, CancellationToken cancellationToken);
        Task DeleteChatAsync(int idChat, CancellationToken cancellationToken);
        Task DeleteMessageAsync(int idMessage, CancellationToken cancellationToken);
    }
}
