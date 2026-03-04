using CodeBattleArena.Server.Data;
using CodeBattleArena.Server.IRepositories;
using CodeBattleArena.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeBattleArena.Server.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly AppDBContext _context;

        public ChatRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task AddChatAsync(string playerId1, string playerId2, CancellationToken cancellationToken)
        {
            if (playerId1 == playerId2)
                return;

            // Упорядочим ID чтобы избежать дубликатов
            var (id1, id2) = string.CompareOrdinal(playerId1, playerId2) < 0
                ? (playerId1, playerId2)
                : (playerId2, playerId1);

            // Проверим, существует ли уже чат
            bool exists = await _context.Chats
                .AnyAsync(f => f.IdPlayer1 == id1 && f.IdPlayer2 == id2, cancellationToken);

            if (exists)
                return;

            var chat = new Chat
            {
                IdPlayer1 = id1,
                IdPlayer2 = id2
            };

            await _context.Chats.AddAsync(chat);
        }
        public async Task AddMessageAsync(int chatId, string senderId, string messageText, CancellationToken cancellationToken)
        {
            var message = new Message
            {
                IdChat = chatId,
                IdSender = senderId,
                MessageText = messageText,
                SentDateTime = DateTime.Now
            };

            await _context.Messages.AddAsync(message);
        }
        public async Task<List<Message>> GetMessagesByChatIdAsync(int chatId, CancellationToken cancellationToken)
        {
            return await _context.Messages
                                 .Where(m => m.IdChat == chatId)
                                 .OrderBy(m => m.SentDateTime)
                                 .ToListAsync(cancellationToken);
        }
        public async Task<Chat> GetChatAsync(int idChat, CancellationToken cancellationToken)
        {
            return await _context.Chats
                                      .Include(c => c.Player1)
                                      .Include(c => c.Player2)
                                      .Include(c => c.Messages)
                                      .FirstOrDefaultAsync(c => c.IdChat == idChat, 
                                      cancellationToken);
        }
        public async Task<Chat> GetChatByIdPlayerAsync(string playerId1, string playerId2, CancellationToken cancellationToken)
        {
            if (playerId1 == playerId2)
                return null;

            var (id1, id2) = string.CompareOrdinal(playerId1, playerId2) < 0
                ? (playerId1, playerId2)
                : (playerId2, playerId1);

            return await _context.Chats
                                      .Include(c => c.Player1)
                                      .Include(c => c.Player2)
                                      .Include(c => c.Messages)
                                      .FirstOrDefaultAsync(c => c.IdPlayer1 == id1 && c.IdPlayer2 == id2,
                                       cancellationToken);
        }
        public async Task<List<Chat>> GetChatsByPlayerIdAsync(string playerId, CancellationToken cancellationToken)
        {
            return await _context.Chats
                     .Where(c => c.IdPlayer1 == playerId || c.IdPlayer2 == playerId)
                     .Include(c => c.Player1)
                     .Include(c => c.Player2)
                     .Include(c => c.Messages)
                     .ToListAsync(cancellationToken);
        }
        public async Task DeleteChatAsync(int idChat, CancellationToken cancellationToken)
        {
            var chat = await _context.Chats.FirstOrDefaultAsync(c => c.IdChat == idChat, cancellationToken);
            if (chat != null) _context.Chats.Remove(chat);
        }
        public async Task DeleteMessageAsync(int idMessage, CancellationToken cancellationToken)
        {
            var message = await _context.Messages.FirstOrDefaultAsync(c => c.IdMessage == idMessage, cancellationToken);
            if (message != null) _context.Messages.Remove(message);
        }
    }
}
