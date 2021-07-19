using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatMicroservice.Data;
using ChatMicroservice.Models;
using ChatMicroservice.IRepository;

namespace ChatMicroservice.Repository
{
    public class MessagesRepository : GenericRepository<Message>, IMessagesRepository
    {
        public MessagesRepository(ChatContext context) : base(context)
        {
        }
    }
}
