using ChatMicroservice.Data;
using ChatMicroservice.DTOs;
using ChatMicroservice.IServices;
using ChatMicroservice.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ChatMicroservice
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IConversationsService conversationService;
        private readonly IHubService hubService;

        //private ChatContext _context;
        //private IHubContext<ChatHub> _chat;
        //private readonly int maxPageSize = 10;

        //public ChatController(ChatContext context, IHubContext<ChatHub> chat)
        //{
        //    _context = context;
        //    _chat = chat;
        //}

        public ChatController(IConversationsService conversationService, IHubService hubService)
        {
            this.conversationService = conversationService;
            this.hubService = hubService;
        }

        [HttpGet]
        [Route("getChats")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetConversations()
        {
            try
            {
                var retVal = await conversationService.GetConversations(User.Claims.First(c => c.Type == "UserID").Value);

                return Ok(retVal);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to return conversations " + e.Message);
                return StatusCode(500, "Failed to return conversations.");
            }
        }

        [HttpGet]
        [Route("getChats/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetConversationById(int id)
        {
            try
            {
                var offset = Request.Headers["X-Timezone-Offset"];
                int clientOffset = 0;

                if (!Int32.TryParse(offset, out clientOffset))
                    return BadRequest();

                //Console.WriteLine("Header: " + header);

                //var queryString = Request.Query;

                //int page = 0;
                //Int32.TryParse(queryString["page"], out page);

                //Console.WriteLine("Page: " + page);

                var retVal = await conversationService.GetConversationById(id, User.Claims.First(c => c.Type == "UserID").Value, clientOffset);

                return Ok(retVal);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to return conversation " + e.Message);

                return StatusCode(500, "Failed to return conversation.");
            }
        }

        [HttpDelete]
        [Route("deleteChat/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> DeleteChat(int id) 
        {
            try
            {
                await conversationService.DeleteConversation(id, User.Claims.First(c => c.Type == "UserID").Value);

                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine("Delete char err: " + e.Message);
                return StatusCode(500, "Something went wrong");
            }
        }
        [HttpPost]
        [Route("sendMessage")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SendMessage(SendMessageDto dto) 
        {
            try
            {

                await conversationService.SendMessage(dto, User.Claims.First(c => c.Type == "UserID").Value);

                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine("Send chat error: " + e.Message);
                return StatusCode(500, "Failed to send msg");
            }
        }

        [HttpPost]
        [Route("sendmsgtonewconv")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SendMessageToNewConversation(SendMessageToNewConversationDto dto)
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                string fname = User.Claims.First(c => c.Type == "FirstName").Value;
                string lname = User.Claims.First(c => c.Type == "LastName").Value;

                await conversationService.SendMessageToNewConversation(dto, userId, fname,lname);

                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine("Send chat error: " + e.Message);
                return StatusCode(500, "Failed to send msg");
            }
        }

        [HttpPost]
        [Route("joinToChat")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> JoinToChat(ChatInfoDto dto) 
        {
            try
            {
                await hubService.JoinToChat(dto);
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failked to join " + e.Message);
                return StatusCode(500, "Something went wrong");
            }
        }

        [HttpPost]
        [Route("leaveChat")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> LeaveChat(ChatInfoDto dto)
        {
            try
            {
                await hubService.LeaveChat(dto);
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failked to leave group " + e.Message);
                return StatusCode(500, "Something went wrong");
            }
        }

        [HttpGet]
        [Route("getChatUsers")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetChatUserIds()
        {
            try
            {
                var msgs = await conversationService.GetChatUserIds(User.Claims.First(c => c.Type == "UserID").Value);

                return Ok(msgs);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to return ids " + e.Message);

                return StatusCode(500, "Something went wrong");
            }
        }
    }
}
