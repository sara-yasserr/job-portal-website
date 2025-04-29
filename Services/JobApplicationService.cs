using Job_Portal_Project.Models;
using Job_Portal_Project.Repositories;

namespace Job_Portal_Project.Services
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly IJobApplicationRepository jobApplicationRebo;

        public JobApplicationService(IJobApplicationRepository _jobApplicationRebo)
        {
            jobApplicationRebo = _jobApplicationRebo;
        }

        public List<JobApplication> GetUserApplications(string userId)
        {
            return jobApplicationRebo.GetUserApplications(userId);
        }

        public void Insert(JobApplication entity)
        {
            entity.ApplicationDate = DateTime.Now;
            jobApplicationRebo.Insert(entity);
            jobApplicationRebo.Save();
        }

        public JobApplication GetJobApplication(int id)
        {
            return jobApplicationRebo.GetById<int>(id);
        }

        public void Delete(int jobId)
        {
            jobApplicationRebo.Delete(jobId);
            jobApplicationRebo.Save();

        }

        public void Update(JobApplication entity)
        {
            jobApplicationRebo.Update(entity);
            jobApplicationRebo.Save();
        }


    }
}
