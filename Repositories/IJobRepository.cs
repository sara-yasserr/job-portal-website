using Job_Portal_Project.Models;
using NPOI.SS.Formula.Functions;

namespace Job_Portal_Project.Repositories
{
    public interface IJobRepository : IRepository<Job>
    {

        IQueryable<Job> GetAllIQ();
    }
}
