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
            throw new NotImplementedException();
        }

        public JobCategory GetById<I>(I id)
        {
            throw new NotImplementedException();
        }

        public void Insert(JobCategory entity)
        {
            throw new NotImplementedException();
        }

        public void Update(JobCategory entity)
        {
            throw new NotImplementedException();
        }
        public void Delete<I>(I id)
        {
            throw new NotImplementedException();
        }
        public void Save()
        {
            throw new NotImplementedException();
        }
    }
}
