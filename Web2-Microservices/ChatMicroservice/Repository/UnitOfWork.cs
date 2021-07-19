using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatMicroservice.Data;
using ChatMicroservice.Models;
using ChatMicroservice.IRepository;

namespace ChatMicroservice.Repository
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {

        private readonly ChatContext _context;
        private IConversationsRepository conversationsRepository;
        private IMessagesRepository messagesRepository;

        public UnitOfWork(ChatContext context)
        {
            _context = context;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task Commit()
        {
            await _context.SaveChangesAsync();
        }

        public void Rollback()
        {
            this.Dispose();
        }

        public IConversationsRepository ConversationsRepository
        {
            get
            {
                return conversationsRepository = conversationsRepository ??
                    new ConversationsRepository(this._context);
            }
        }

        public IMessagesRepository MessagesRepository
        {
            get
            {
                return messagesRepository = messagesRepository ??
                    new MessagesRepository(this._context);
            }
        }
    }
}
