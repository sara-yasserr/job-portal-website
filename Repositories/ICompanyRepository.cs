using Job_Portal_Project.Models;

namespace Job_Portal_Project.Repositories
{
    public interface ICompanyRepository : IRepository<Company>
    {
        public List<Company> FilterCompanies(string name, string city, string country);

    }
}
