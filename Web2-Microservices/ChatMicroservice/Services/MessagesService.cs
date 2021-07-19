using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatMicroservice.Models;
using ChatMicroservice.Repository;
using ChatMicroservice.DTOs;
using ChatMicroservice.IServices;
using ChatMicroservice.IRepository;

namespace ChatMicroservice.Services
{
    public class MessagesService : IMessagesService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MessagesService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork; 
        }
      
    }
}
