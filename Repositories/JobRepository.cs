using Job_Portal_Project.Models;
using Job_Portal_Project.Models.DbContext;

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
            return _context.Jobs.ToList();
        }
        public Job GetById<I>(I id)
        {
            return _context.Jobs.Find(id);
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
            var job = _context.Jobs.Find(id);
            if (job != null)
            {
                _context.Jobs.Remove(job);
            }
        }
        public void Save()
        {
            _context.SaveChanges();
        }
        public int Count()
        {
            return _context.Jobs.Count();
        }
    }
}
