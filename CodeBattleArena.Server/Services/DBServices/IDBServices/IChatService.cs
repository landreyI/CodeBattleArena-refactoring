using CodeBattleArena.Server.Models;

namespace CodeBattleArena.Server.Services.DBServices.IDBServices
{
    public interface IChatService
    {
        Task<bool> AddChatAsync(string playerId1, string playerId2, CancellationToken cancellationToken, bool commit = true);
        Task<bool> AddMessageAsync(int chatId, string senderId, string messageText, CancellationToken cancellationToken, bool commit = true);
        Task<List<Message>> GetMessagesByChatIdAsync(int chatId, CancellationToken cancellationToken);
        Task<Chat> GetChatAsync(int idChat, CancellationToken cancellationToken);
        Task<Chat> GetChatByIdPlayerAsync(string playerId1, string playerId2, CancellationToken cancellationToken);
        Task<List<Chat>> GetChatsByPlayerIdAsync(string playerId, CancellationToken cancellationToken);
        Task<bool> DeleteChatAsync (int idChat, CancellationToken cancellationToken, bool commit = true);
        Task<bool> DeleteMessageAsync(int idMessage, CancellationToken cancellationToken, bool commit = true);
    }
}
