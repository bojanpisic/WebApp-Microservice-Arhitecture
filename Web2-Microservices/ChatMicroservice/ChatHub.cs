using ChatMicroservice.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatMicroservice
{
    public class ChatHub: Hub
    {
        private static Dictionary<string, string> connectedUsers = new Dictionary<string, string>();


        public override Task OnConnectedAsync()
        {
            //string connectionId = Context.ConnectionId;
            //string connectedClientId = Context.GetHttpContext().Request.Query["userId"];

            //if(!connectedUsers.ContainsKey(connectedClientId))
            //    connectedUsers.Add(connectedClientId,connectionId);

            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            //Console.WriteLine("DISCONNECTED: " + Context.ConnectionId);
            //connectedUsers.Remove(connectedUsers.First(usr => usr.Value.Equals(Context.ConnectionId)).Key);
            return base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessage(string req)
        {
            //var routeOb = JsonConvert.DeserializeObject<dynamic>(req);
            //Console.WriteLine(req);

            //Message msg = new Message() 
            //{

            //};

            //var connectionId = connectedUsers[routeOb.friendId];
            //await Clients.Client(connectionId)
            //    .SendAsync("SendMessageAsync", msg);
        }

        public string GetConnectionId() => Context.ConnectionId;

        public async Task OnTyping(string roomName, string senderId) 
        {
            await Clients.Group(roomName).SendAsync("typing", senderId);
        }
        public async Task StopTyping(string roomName, string senderId)
        {
            await Clients.Group(roomName).SendAsync("stopTyping", senderId);
        }
    }
}
