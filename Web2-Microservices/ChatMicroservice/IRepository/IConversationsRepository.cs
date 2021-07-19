using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatMicroservice.DTOs;
using ChatMicroservice.Models;

namespace ChatMicroservice.IRepository
{
    public interface IConversationsRepository : IGenericRepository<Conversation>
    {
        Task<IEnumerable<object>> GetConversations(string userId);
        Task<object> GetConversationById(int id, string userId, int clientOffset);
        Task DeleteConversation(int id, string userId);
        Task<Tuple<string, string>> SendMessage(SendMessageDto dto, string senderId);
        Task<Tuple<string, string>> SendMessageToNewConversation(SendMessageToNewConversationDto dto, string senderId, string senderFirstName, string senderLastName);
        Task< IEnumerable<object>> GetChatUserIds(string userId);
    }
}
