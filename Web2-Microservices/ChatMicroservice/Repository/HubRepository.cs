using ChatMicroservice.Data;
using ChatMicroservice.DTOs;
using ChatMicroservice.IRepository;
using ChatMicroservice.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatMicroservice.Repository
{
    public class HubRepository: IHubRepository
    {
        private readonly IHubContext<ChatHub> _chat;

        public HubRepository(IHubContext<ChatHub> chat)
        {
            _chat = chat;

        }

        public async Task JoinToChat(ChatInfoDto dto)
        {
            await _chat.Groups.AddToGroupAsync(dto.connectionId, dto.roomName);
        }

        public async Task LeaveChat(ChatInfoDto dto)
        {
            await _chat.Groups.RemoveFromGroupAsync(dto.connectionId, dto.roomName);
        }

        public async Task SendMessage(string message, string roomName)
        {
            await _chat.Clients.Group(roomName).SendAsync("ReceiveMessage", message);

        }

        public async Task SendNotification(string notification)
        {
            await _chat.Clients.All.SendAsync("Notfication", notification);
        }
    }
}
