// ViewModels/ManageAccountViewModel.cs
namespace Job_Portal_Project.ViewModels
{
    public class ManageAccountViewModel
    {
        public ProfileViewModel Profile { get; set; }
        public int NotificationCount { get; set; }
        public int BookmarkedJobsCount { get; set; }
        public int ApplicationsCount { get; set; }
    }
}
