using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatMicroservice.Models;
using ChatMicroservice.IRepository;

namespace ChatMicroservice.IRepository
{
    public interface IUnitOfWork
    {
        IConversationsRepository ConversationsRepository {get;}
        IMessagesRepository MessagesRepository {get;}

        Task Commit();
        void Rollback();
    }
}
