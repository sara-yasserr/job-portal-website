using Job_Portal_Project.Models;
using Job_Portal_Project.Models.DbContext;

namespace Job_Portal_Project.Repositories
{
    public class JobApplicationRepository : IJobApplicationRepository
    {
        private readonly JobPortalContext _context;
        public JobApplicationRepository(JobPortalContext context) 
        {
            _context = context;
        }
        public List<JobApplication> GetAll()
        {
            return _context.JobApplications.ToList();
        }
        public JobApplication GetById<I>(I id)
        {
            return _context.JobApplications.Find(id);
        }
        public void Insert(JobApplication entity)
        {
            _context.JobApplications.Add(entity);
        }
        public void Update(JobApplication entity)
        {
            _context.JobApplications.Update(entity);
        }
        public void Delete<I>(I id)
        {
            var jobApplication = _context.JobApplications.Find(id);
            if (jobApplication != null)
            {
                _context.JobApplications.Remove(jobApplication);
            }
        }
        public void Save()
        {
            _context.SaveChanges();
        }
        public int Count()
        {
            return _context.JobApplications.Count();
        }

    }
}
