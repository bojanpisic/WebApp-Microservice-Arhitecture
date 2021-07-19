using ChatMicroservice.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatMicroservice.IServices
{
    public interface IHubService
    {
        Task JoinToChat(ChatInfoDto dto);
        Task LeaveChat(ChatInfoDto dto);
    }
}
