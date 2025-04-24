using Job_Portal_Project.Models;
using Job_Portal_Project.Models.DbContext;

namespace Job_Portal_Project.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly JobPortalContext _context;
        public CompanyRepository(JobPortalContext context)
        {
            _context = context;
        }
        public List<Company> GetAll()
        {
            return _context.Companies.ToList();
        }
        public Company GetById<I>(I id)
        {
            return _context.Companies.Find(id);
        }
        public void Insert(Company entity)
        {
            _context.Companies.Add(entity);
        }
        public void Update(Company entity)
        {
            _context.Companies.Update(entity);
        }
        public void Delete<I>(I id)
        {
            var company = _context.Companies.Find(id);
            if (company != null)
            {
                _context.Companies.Remove(company);
            }
        }
        public void Save()
        {
            _context.SaveChanges();
        }
        public List<Company> FilterCompanies(string name, string city, string country)
        {
            var query = _context.Companies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(c => c.Name.ToLower().Contains(name.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(c => c.City.ToLower().Contains(city.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(country))
            {
                query = query.Where(c => c.Country.ToLower().Contains(country.ToLower()));
            }

            return query.ToList();

        int IRepository<Company>.Count()
        {
            return _context.Companies.Count();
        }
    }
}
