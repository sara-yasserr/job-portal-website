using Job_Portal_Project.Models;

namespace Job_Portal_Project.Services
{
    public interface IJobService
    {
        List<Job> GetAllJobs();
        Job GetJobById(int id);
        void CreateJob(Job job);
        void UpdateJob(Job job);
        void DeleteJob(int id);
    }
}
