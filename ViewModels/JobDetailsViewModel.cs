using Job_Portal_Project.Models;

namespace Job_Portal_Project.ViewModels
{
    public class JobDetailsViewModel
    {
        public Job Job { get; set; }
        public string CurrentUserId { get; set; }
        public ApplicationUser CurrentUser { get; set; }
        public bool HasApplied { get; set; }
        public bool IsBookmarked { get; set; }
        public List<Job> RelatedJobs { get; set; } = new List<Job>();



    }
}