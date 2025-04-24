using Job_Portal_Project.Models;

namespace Job_Portal_Project.ViewModels.Admin
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalJobs { get; set; }
        public int TotalActiveJobs { get; set; }
        public int TotalInactiveJobs { get; set; }
        public int TotalCompanies { get; set; }
        public List<CompanyJobCountViewModel> TopCompaniesByJobs { get; set; }
        public int TotalApplications { get; set; }
        public int TotalEmployers { get; set; }
        public int TotalApplicants { get; set; }
        public int TotalCategories { get; set; }
        public string MostActiveCategoryName { get; set; }
        public int MostActiveCategoryJobCount { get; set; }
        public List<ApplicationUser> Users { get; set; } = new();
        public List<Job> Jobs { get; set; } = new();
        public List<Company> Companies { get; set; } = new();
        public List<JobApplication> Applications { get; set; } = new();
        public List<JobCategory> Categories { get; set; } = new();
    }
}
