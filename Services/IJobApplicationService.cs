using Job_Portal_Project.Models;

namespace Job_Portal_Project.Services
{
    public interface IJobApplicationService
    {
        public List<JobApplication> GetUserApplications(string userId);
        public void Insert(JobApplication entity);
        public JobApplication GetJobApplication(int id);
        public void Delete(int jobId);
        public void Update(JobApplication entity);


    }
}
