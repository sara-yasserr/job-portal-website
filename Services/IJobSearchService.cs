// Services/Contracts/IJobSearchService.cs
using Job_Portal_Project.Models;
using Job_Portal_Project.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Job_Portal_Project.Services.Contracts
{
    public interface IJobSearchService
    {
        Task<JobSearchViewModel> SearchJobs(JobSearchViewModel model);
        Task<List<Job>> GetRecentJobs(int count = 6);
        Task<Job> GetJobDetails(int jobId);
        Task<ApplicationUser> GetCurrentUser(string userId);
        Task<bool> HasUserApplied(string userId, int jobId);
        Task<List<Job>> GetRelatedJobs(int categoryId, int excludeJobId);
        Task<List<CategoryWithJobCount>> GetAllCategories();
        Task<bool> ToggleFavoriteJob(string userId, int jobId);
    }
}