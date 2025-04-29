using Job_Portal_Project.Models;

namespace Job_Portal_Project.ViewModels
{
    public class JobDetailsViewModel
    {
        public Job Job { get; set; }
        public ApplicationUser CurrentUser { get; set; }
        public bool HasApplied { get; set; }
        public bool IsFavorite { get; set; }
        public List<Job> RelatedJobs { get; set; } = new List<Job>();

    }
}
