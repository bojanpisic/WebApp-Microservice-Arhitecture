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
    public class ConversationsService : IConversationsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubRepository hubRepository;

        public ConversationsService(IUnitOfWork unitOfWork, IHubRepository hubRepository)
        {
            this._unitOfWork = unitOfWork;
            this.hubRepository = hubRepository;
        }

        public async Task DeleteConversation(int id, string userId)
        {
            try
            {
                await _unitOfWork.ConversationsRepository.DeleteConversation(id, userId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<object> GetChatUserIds(string userId)
        {
            try
            {
                return await _unitOfWork.ConversationsRepository.GetChatUserIds(userId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<object> GetConversationById(int id, string userId, int clientOffset)
        {
            try
            {
                return await _unitOfWork.ConversationsRepository.GetConversationById(id, userId, clientOffset);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<object>> GetConversations(string userId)
        {
            try
            {
                return await _unitOfWork.ConversationsRepository.GetConversations(userId);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SendMessage(SendMessageDto dto, string userId)
        {
            try
            {
                Tuple<string, string> retVal = await _unitOfWork.ConversationsRepository.SendMessage(dto, userId);
                await hubRepository.SendMessage(retVal.Item1, dto.RoomName);
                await hubRepository.SendNotification(retVal.Item2);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task SendMessageToNewConversation(SendMessageToNewConversationDto dto, string userId, string firstName, string lastName)
        {
            try
            {
                Tuple<string, string> retVal = await _unitOfWork.ConversationsRepository.SendMessageToNewConversation(dto, userId, firstName, lastName);
                await hubRepository.SendMessage(retVal.Item1, dto.RoomName);
                await hubRepository.SendNotification(retVal.Item2);
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
