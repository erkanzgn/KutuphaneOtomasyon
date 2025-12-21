using Kutuphane.Application.Dtos.ContactDtos;

namespace Kutuphane.WebUI.Models.ViewModels
{
    public class ContactListViewModel
    {
        public List<ResultContactDto> InboxMessages { get; set; } 
        public List<ResultContactDto> OutboxMessages { get; set; } 
    }
}
