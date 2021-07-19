using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatMicroservice.Models;
using ChatMicroservice.DTOs;

namespace ChatMicroservice.IServices
{
    public interface IConversationsService
    {
        Task<IEnumerable<object>> GetConversations(string userId);
        Task<object> GetConversationById(int id, string userId, int clientOffset);
        Task DeleteConversation(int id, string userId);
        Task SendMessage(SendMessageDto  dto, string userId);
        Task SendMessageToNewConversation(SendMessageToNewConversationDto dto, string userId , string firstName, string lastName);
        Task<object> GetChatUserIds(string userId);

    }
}
