using ChatMicroservice.DTOs;
using ChatMicroservice.IRepository;
using ChatMicroservice.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatMicroservice.Services
{
    public class HubService : IHubService
    {
        private readonly IHubRepository hubRepository;

        public HubService(IHubRepository hubRepository)
        {
            this.hubRepository = hubRepository;
        }
        public async Task JoinToChat(ChatInfoDto dto)
        {
            await hubRepository.JoinToChat(dto);
        }

        public async Task LeaveChat(ChatInfoDto dto)
        {
            await hubRepository.LeaveChat(dto);
        }
    }
}
