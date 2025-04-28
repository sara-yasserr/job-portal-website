using Job_Portal_Project.Models;
using Job_Portal_Project.ViewModels;

namespace Job_Portal_Project.Services
{
    public interface IJobApplicationService
    {
        public List<JobApplication> GetUserApplications(string userId);
        public JobApplication GetJobApplication(int id);
        public void Insert(JobApplicationViewModel entity);
        public void Delete<T>(T jobId);
        public void Update(JobApplicationViewModel entity);

    }
}
