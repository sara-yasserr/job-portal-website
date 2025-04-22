using Job_Portal_Project.Models;
using Job_Portal_Project.Models.DbContext;

namespace Job_Portal_Project.Repositories
{
    public class JobCategoryRepository : IJobCategoryRepository
    {
        private readonly JobPortalContext _context;
        public JobCategoryRepository(JobPortalContext context)
        {
            _context = context;
        }

        public List<JobCategory> GetAll()
        {
            return _context.JobCategories.ToList();
        }

        public JobCategory GetById<I>(I id)
        {
            return _context.JobCategories.Find(id);
        }

        public void Insert(JobCategory entity)
        {
            _context.JobCategories.Add(entity);
        }

        public void Update(JobCategory entity)
        {
            _context.JobCategories.Update(entity);
        }
        public void Delete<I>(I id)
        {
            var Record = _context.JobCategories.Find(id);
            _context.JobCategories.Remove(Record);
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
