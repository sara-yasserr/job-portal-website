using Job_Portal_Project.Models;
using Job_Portal_Project.ViewModels;

namespace Job_Portal_Project.Services
{
    public interface IJobApplicationService : IService<JobApplicationViewModel>
    {
        public List<JobApplication> GetUserApplications(string userId);
        public JobApplication GetJobApplication(int id);


    }
}
