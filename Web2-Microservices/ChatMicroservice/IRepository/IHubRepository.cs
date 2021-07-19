using ChatMicroservice.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatMicroservice.IRepository
{
    public interface IHubRepository
    {
        Task SendMessage(string message, string roomName);
        Task SendNotification(string notification);
        Task JoinToChat(ChatInfoDto dto);
        Task LeaveChat(ChatInfoDto dto);
    }
}
