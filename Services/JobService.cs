using Job_Portal_Project.Models;
using Job_Portal_Project.Repositories;

namespace Job_Portal_Project.Services
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _jobRepository;

        public JobService(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }
        public void CreateJob(Job job)
        {
            _jobRepository.Insert(job);
            _jobRepository.Save();
        }

        public void DeleteJob(int id)
        {
            _jobRepository.Delete(id);
            _jobRepository.Save();
        }

        public List<Job> GetAllJobs()
        {
            return _jobRepository.GetAll();
        }

        public IQueryable<Job> GetAllJobsIQ()
        {
            return _jobRepository.GetAllIQ();   
        }

        public Job GetJobById(int id)
        {
           return _jobRepository.GetById(id);
        }

        public void UpdateJob(Job job)
        {
            _jobRepository.Update(job);
            _jobRepository.Save();
        }
    }
}
