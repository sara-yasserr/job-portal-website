using Job_Portal_Project.Models;

namespace Job_Portal_Project.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalJobs { get; set; }
        public int TotalCompanies { get; set; }
        public int TotalApplications { get; set; }
        public List<ApplicationUser> RecentUsers { get; set; } = new List<ApplicationUser>();
        public List<Job> RecentJobs { get; set; } = new List<Job>();
        public List<Company> RecentCompanies { get; set; } = new List<Company>();
        public List<JobApplication> RecentApplications { get; set; } = new List<JobApplication>();
    }
}
