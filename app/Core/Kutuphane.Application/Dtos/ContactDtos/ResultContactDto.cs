
namespace Kutuphane.Application.Dtos.ContactDtos
{
    public class ResultContactDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsRead { get; set; }
        public string? ReplyMessage { get; set; }

       
        public string MessageType { get; set; }
    }
}