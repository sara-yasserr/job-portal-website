using Job_Portal_Project.Models;
using Job_Portal_Project.Models.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_Project.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly JobPortalContext _context;
        public JobRepository(JobPortalContext context)
        {
            _context = context;
        }


        public List<Job> GetAll()
        {
            return _context.Jobs.Include(j => j.Company).Include(j => j.JobCategory).ToList();
        }

        public IQueryable<Job> GetAllIQ()
        {
            return _context.Jobs
                .Include(j => j.Company)
                .Include(j => j.JobCategory)
                .AsQueryable(); 
        }





        public Job? GetById<I>(I id)
        {
            return _context.Jobs
                   .Include(j => j.Company)
                   .Include(j => j.JobCategory)
                   .FirstOrDefault(j => j.Id.Equals(id));
        }

        public void Insert(Job entity)
        {
            _context.Jobs.Add(entity);
        }
        public void Update(Job entity)
        {
            _context.Jobs.Update(entity);
        }
        public void Delete<I>(I id)
        {
            var job = GetById(id);
            if (job != null)
                _context.Jobs.Remove(job);
        }
        public void Save()
        {
            _context.SaveChanges();
        }

        public int Count()
        {
            return _context.Jobs.Count();
        }

        public int GetVacancyCount(int jobId)
        {
            return _context.Jobs
                .Where(job => job.Id == jobId)
                .Select(job => job.VacancyCount)
                .FirstOrDefault();
        }
    }
}
