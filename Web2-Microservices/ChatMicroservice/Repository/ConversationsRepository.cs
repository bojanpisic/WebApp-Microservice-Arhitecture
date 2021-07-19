using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatMicroservice.Data;
using ChatMicroservice.Models;
using ChatMicroservice.IRepository;
using ChatMicroservice.Repository;
using ChatMicroservice.DTOs;
using Newtonsoft.Json;

namespace ChatMicroservice.Repository
{
    public class ConversationsRepository : GenericRepository<Conversation>, IConversationsRepository
    {
        public ConversationsRepository(ChatContext context) : base(context)
        {
        }

        public async Task DeleteConversation(int id, string userId)
        {
            var conv = await context.Conversations
                   .FirstOrDefaultAsync(x => x.ConversationId.Equals(id));

            if (conv.ReceiverId.Equals(userId))
            {
                conv.ReceiverDeleteTime = DateTime.Now;
            }
            else
            {
                conv.SenderDeleteTime = DateTime.Now;
            }

            context.Conversations.Update(conv);

            await context.SaveChangesAsync();

        }

        public async Task<IEnumerable<object>> GetChatUserIds(string userId)
        {

            var conversations = await context.Conversations
                        .Include(x => x.Messages)
                        .Where(x => x.ReceiverId.Equals(userId) || x.SenderId.Equals(userId))
                        .ToListAsync();

            var ids = new List<object>();
            foreach (var conversation in conversations)
            {
                if(conversation.ReceiverId.Equals(userId)?
                        conversation.ReceiverDeleteTime != DateTime.MinValue
                         && conversation.Messages
                                        .LastOrDefault().TimeStamp < conversation.ReceiverDeleteTime : 
                        conversation.SenderDeleteTime != DateTime.MinValue
                         && conversation.Messages
                                        .LastOrDefault().TimeStamp < conversation.SenderDeleteTime)
                    continue;
                ids.Add(new
                {
                    Id = conversation.ReceiverId.Equals(userId) ? conversation.SenderId : conversation.ReceiverId
                });
            }

            return ids;

        }

        public async Task<object> GetConversationById(int id, string userId, int clientOffset)
        {

            var conv = await context.Conversations
                                  .FirstOrDefaultAsync(x => x.ConversationId.Equals(id));

            var messages = await context.Messages
                .OrderByDescending(m => m.TimeStamp)
                //.Skip(page * maxPageSize)
                //.Take(maxPageSize)
                .Where(m => m.ConversationId.Equals(conv.ConversationId))
                .ToListAsync();

            var msgs = new List<object>();
            foreach (var msg in messages)
            {
                if ((userId.Equals(conv.ReceiverId) ? conv.ReceiverDeleteTime : conv.SenderDeleteTime) >= msg.TimeStamp)
                {
                    continue;
                }

               

                msgs.Add(new
                {
                    text = msg.Text,
                    timeStamp = ((DateTimeOffset)msg.TimeStamp).ToOffset(new TimeSpan(clientOffset, 0, 0)).DateTime,
                    senderId = msg.SenderId,
                    received = msg.Received
                });
            }

            return new
            {
                conversationId = conv.ConversationId,
                roomName = conv.RoomName,
                friendFirstName = userId.Equals(conv.ReceiverId) ? conv.SenderFirstname : conv.ReceiverFirstname,
                friendLastName = userId.Equals(conv.ReceiverId) ? conv.SenderLastName : conv.ReceiverLastName,
                receiverId = conv.ReceiverId,
                messages = msgs
            };

        }

        public async Task<IEnumerable<object>> GetConversations(string userId)
        {

            var conv = await context.Conversations
                                   .Include(x => x.Messages)
                                   .Where(x => x.ReceiverId.Equals(userId) || x.SenderId.Equals(userId))
                                   .ToListAsync();


            var retVal = new List<object>();

            foreach (var item in conv.OrderByDescending(c => c.Messages.LastOrDefault().TimeStamp))
            {
                if ((userId.Equals(item.ReceiverId) ? item.ReceiverDeleteTime : item.SenderDeleteTime) >= item.Messages.LastOrDefault().TimeStamp)
                {
                    continue;
                }

                retVal.Add(new
                {
                    conversationId = item.ConversationId,
                    roomName = item.RoomName,
                    friendFirstName = userId.Equals(item.ReceiverId) ? item.SenderFirstname : item.ReceiverFirstname,
                    friendLastName = userId.Equals(item.ReceiverId) ? item.SenderLastName : item.ReceiverLastName,
                    message = item.Messages.LastOrDefault().Text,
                    sender = item.Messages.LastOrDefault().SenderId,
                });
            }

            return retVal;

        }

        private int GetServerTimeOffset(DateTimeOffset localTime) 
        {
            return (-localTime.Offset.Hours);
        }

        public async Task<Tuple<string, string>> SendMessage(SendMessageDto dto, string senderId)
        {
            DateTimeOffset localTime = new DateTimeOffset(DateTime.Now);

            var conversation = context.Conversations
                .FirstOrDefault(x => x.ConversationId.Equals(dto.ConversationId));

            var msg = new Message()
            {
                Text = dto.Message,
                TimeStamp = localTime.DateTime,
                Received = true,
                SenderId = senderId
            };

            conversation.Messages.Add(msg);

            await context.SaveChangesAsync();

            var msgToSend = JsonConvert.SerializeObject(msg, Newtonsoft.Json.Formatting.Indented,
                                new JsonSerializerSettings()
                                {
                                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                                }
                            );


            var notificationInfo = new
            {
                conversationId = conversation.ConversationId,
                text = msg,
                // senderId = conversation.SenderId,
                // receiverId = conversation.ReceiverId,
                senderFirstName = senderId.Equals(conversation.SenderId) ? conversation.SenderFirstname : conversation.ReceiverFirstname,
                senderLastName = senderId.Equals(conversation.SenderId) ? conversation.SenderLastName : conversation.ReceiverLastName,
                roomName = conversation.RoomName,
            };
            var notification = JsonConvert.SerializeObject(notificationInfo, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                }
            );

            return new Tuple<string, string>(msgToSend, notification);


        }

        public async Task<Tuple<string, string>> SendMessageToNewConversation(SendMessageToNewConversationDto dto, string senderId, string senderFirstName, string senderLastName)
        {
            Conversation conversation;

            var existingConversation = await context.Conversations
                                                .Where(c => (c.SenderId.Equals(senderId) && c.ReceiverId.Equals(dto.ReceiverId)) 
                                                            || (c.SenderId.Equals(dto.ReceiverId) && c.ReceiverId.Equals(senderId)))
                                                .FirstOrDefaultAsync();

            Console.WriteLine("Existing conversation = " + existingConversation);

            if (existingConversation != null)
            {
                conversation = existingConversation;
            }
            else 
            {
                conversation = new Conversation()
                {
                    ReceiverFirstname = dto.ReceiverFirstName,
                    ReceiverLastName = dto.ReceiverLastName,
                    SenderFirstname = senderFirstName,
                    SenderLastName = senderLastName,
                    ReceiverId = dto.ReceiverId,
                    SenderId = senderId,
                    RoomName = dto.RoomName
                };
				
				await context.Conversations.AddAsync(conversation);
            }

            DateTimeOffset localTime = new DateTimeOffset(DateTime.Now);


            var msg = new Message()
            {
                Text = dto.Message,
                TimeStamp = localTime.DateTime,
                Received = true,
                SenderId = senderId
            };

            conversation.Messages.Add(msg);

            await context.SaveChangesAsync();

            var msgToSend = JsonConvert.SerializeObject(msg, Formatting.Indented,
                                new JsonSerializerSettings()
                                {
                                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                                }
                            );

            var notificationInfo = new
            {
                conversationId = conversation.ConversationId,
                text = msg,
                senderId = conversation.SenderId,
                receiverId = conversation.ReceiverId,
                senderFirstName = conversation.SenderFirstname,
                senderLastName = conversation.SenderLastName,
                roomName = conversation.RoomName,
            };
            var notification = JsonConvert.SerializeObject(notificationInfo, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                }
            );

            return new Tuple<string, string>(msgToSend, notification);
        }
    }
}
