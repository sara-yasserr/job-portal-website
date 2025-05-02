using Job_Portal_Project.Repositories;
using Job_Portal_Project.Repositories.ApplicationUserRepository;
using Job_Portal_Project.ViewModels.Admin;

namespace Job_Portal_Project.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IJobRepository _jobRepository;
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IJobCategoryRepository _categoryRepository;
        private readonly IFavouritesRepository _favouritesRepository;

        public AdminDashboardService(IFavouritesRepository favouritesRepository, ICompanyRepository companyRepository,
            IJobApplicationRepository jobApplicationRepository, IJobRepository jobRepository,
            IApplicationUserRepository applicationUserRepository, IJobCategoryRepository jobCategoryRepository)
        {
            _favouritesRepository = favouritesRepository;
            _companyRepository = companyRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _jobRepository = jobRepository;
            _applicationUserRepository = applicationUserRepository;
            _categoryRepository = jobCategoryRepository;
        }

        public async Task<AdminDashboardViewModel> GetDashboardDataAsync()
        {
            var allJobs = _jobRepository.GetAll().ToList();
            var categories = _categoryRepository.GetAll().ToList();
            var mostActiveCategory = allJobs
            .GroupBy(j => j.JobCategoryId)
            .Select(g => new
            {
                CategoryId = g.Key,
                JobCount = g.Count()
            })
            .OrderByDescending(g => g.JobCount)
            .FirstOrDefault();

            var mostActiveCategoryName = categories
            .FirstOrDefault(c => c.Id == mostActiveCategory?.CategoryId)?.Name ?? "N/A";

            var topCompanies = allJobs
            .GroupBy(j => j.CompanyId)
            .Select(g => new
            {
                CompanyId = g.Key,
                JobCount = g.Count()
            })
            .OrderByDescending(g => g.JobCount)
            .Take(5)
            .ToList();

            var companies = _companyRepository.GetAll().ToList();

            var topCompanyViewModels = topCompanies
                .Select(tc =>
                {
                    var company = companies.FirstOrDefault(c => c.Id == tc.CompanyId);
                    return new CompanyJobCountViewModel
                    {
                        CompanyName = company?.Name ?? "Unknown",
                        JobCount = tc.JobCount,
                        LogoPath = company?.LogoPath
                    };
                }).ToList();
            return new AdminDashboardViewModel
            {
                TotalJobs = _jobRepository.Count(),
                TotalActiveJobs = allJobs.Count(j => j.IsActive),
                TotalInactiveJobs = allJobs.Count(j => !j.IsActive),
                TotalApplications = _jobApplicationRepository.Count(),
                TotalCompanies = _companyRepository.Count(),
                TopCompaniesByJobs = topCompanyViewModels,
                TotalUsers = _applicationUserRepository.GetNumberOfUsersAsync().Result,
                TotalApplicants = _applicationUserRepository.GetNumberOfApplicantsAsync().Result,
                TotalEmployers = _applicationUserRepository.GetNumberOfEmployersAsync().Result,
                TotalCategories = _categoryRepository.Count(),
                MostActiveCategoryName = mostActiveCategoryName,
                MostActiveCategoryJobCount = mostActiveCategory?.JobCount ?? 0,
                Users = await _applicationUserRepository.GetAllAsync(),
                Jobs = _jobRepository.GetAll().ToList(),
                Companies = _companyRepository.GetAll().ToList(),
                Applications = _jobApplicationRepository.GetAll().ToList(),
                Categories = _categoryRepository.GetAll().ToList()


            };
        }
    }
}
