using Kutuphane.Application.Dtos.ContactDtos;
using Kutuphane.Application.Interfaces.Repositories;
using Kutuphane.Application.Interfaces.Services;
using Kutuphane.Domain.Entities;

namespace Kutuphane.Application.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactMessageRepository _contactRepository;

        public ContactService(IContactMessageRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task<IEnumerable<ContactMessage>> GetUnreadMessagesAsync()
        {
            return await  _contactRepository.GetUnreadMessagesAsync();
        }

        public async Task SendMessageAsync(ContactMessageDto dto, int? userId)
        {
            var message = new ContactMessage
            {
                Name = dto.Name,
                Email = dto.Email,
                Subject = dto.Subject,
                MessageType = dto.MessageType,
                Message = dto.Message,
                UserId = userId,
                CreatedDate = DateTime.Now,
                IsRead = false
            };

            await _contactRepository.AddAsync(message);
        }
        public async Task ReplyToMessageAsync(int id, string replyMessage)
        {
            var message = await _contactRepository.GetByIdAsync(id);
            if (message != null)
            {
                message.AdminReplay = replyMessage;   
                message.ReplayDate = DateTime.Now;   
                message.IsRead = true;                

                await _contactRepository.UpdateAsync(message);
            }
        }

        public async Task<IEnumerable<ResultContactDto>> GetMessagesByUserIdAsync(int userId)
        {
            var messages = await _contactRepository.GetMessagesByUserIdAsync(userId);

         
            return messages.Select(x => new ResultContactDto
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                Subject = x.Subject,
                Message = x.Message,
                CreatedDate = x.CreatedDate,
                IsRead = x.IsRead,
                ReplyMessage = x.AdminReplay
            }).ToList();
        }
      

        public async Task<IEnumerable<ResultContactDto>> GetMessagesByEmailAsync(string email)
        {
    
            var allMessages = await _contactRepository.GetAllAsync();

      
            var userMessages = allMessages
                .Where(x => x.Email == email)
                .OrderByDescending(x => x.CreatedDate)
                .ToList();

            return userMessages.Select(x => new ResultContactDto
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                Subject = x.Subject,
                Message = x.Message,
                CreatedDate = x.CreatedDate,
                IsRead = x.IsRead,
                ReplyMessage = x.AdminReplay,
                MessageType = x.MessageType.ToString() 
            }).ToList();
        }

        public async Task<IEnumerable<ResultContactDto>> GetAllMessagesAsync()
        {
            var messages = await _contactRepository.GetAllAsync();

            
            return messages.Select(x => new ResultContactDto
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                Subject = x.Subject,
                Message = x.Message,
                CreatedDate = x.CreatedDate,
                IsRead = x.IsRead,
                ReplyMessage = x.AdminReplay, 

                MessageType = x.MessageType.ToString()
            })
            .OrderByDescending(x => x.CreatedDate)
            .ToList();
        }

        public async Task<ContactMessage> GetMessageByIdAsync(int id)
        {
            return await _contactRepository.GetByIdAsync(id);
        }

    
        public async Task MarkAsReadAsync(int id)
        {
            var message = await _contactRepository.GetByIdAsync(id);
            if (message != null && !message.IsRead)
            {
                message.IsRead = true;
                await _contactRepository.UpdateAsync(message);
            }
        }

       
        public async Task DeleteMessageAsync(int id)
        {
            var message = await _contactRepository.GetByIdAsync(id);
            if (message != null)
            {
                message.IsDeleted = true;
                await _contactRepository.UpdateAsync(message);
            }
        }
    }
}