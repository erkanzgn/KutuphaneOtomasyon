namespace Kutuphane.WebUI.Models.ViewModels
{
    public class AboutViewModel
    {
        public int TotalBooks { get; set; }
        public int TotalMembers { get; set; }
        public int ActiveLoans { get; set; }
 
        public List<TeamMemberViewModel> TeamMembers { get; set; } = new List<TeamMemberViewModel>();
    }

    public class TeamMemberViewModel
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; } 
    }
}