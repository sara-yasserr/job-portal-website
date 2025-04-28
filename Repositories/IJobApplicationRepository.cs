using Job_Portal_Project.Models;

namespace Job_Portal_Project.Repositories
{
    public interface IJobApplicationRepository : IRepository<JobApplication>
    {
        public List<JobApplication> GetUserApplications(string userId);
        public JobApplication GetByJobIdAndApplicantId(int jobId, string applicantId);

    }
}
