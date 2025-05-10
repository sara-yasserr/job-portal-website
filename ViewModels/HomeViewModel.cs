using Job_Portal_Project.Models;

namespace Job_Portal_Project.ViewModels
{
    public class HomeViewModel
    {
        public List<CategoryWithJobCount> Categories { get; set; }
        public List<Job> LatestJobs { get; set; } = new();
    }
}
