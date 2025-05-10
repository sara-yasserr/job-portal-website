using Job_Portal_Project.Models;

namespace Job_Portal_Project.ViewModels
{
    public class JobFilterViewModel
    {
        public string? Title { get; set; }
        public string? CompanyName { get; set; }
        public int? CategoryId { get; set; }

        public List<Job> Jobs { get; set; } = new();

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<JobCategory> Categories { get; set; } = new List<JobCategory>();

    }
}
