using Job_Portal_Project.Repositories;
using Job_Portal_Project.Repositories.ApplicationUserRepository;
using Microsoft.AspNetCore.Mvc;

namespace Job_Portal_Project.Controllers
{
    public class Admin : Controller
    {   
        private ICompanyRepository _companyRepository;
        private IJobRepository _jobRepository;
        private IJobApplicationRepository _jobApplicationRepository;
        private IApplicationUserRepository _applicationUserRepository;
        private IJobCategoryRepository _categoryRepository;

        public Admin()
        {
            
        }
        public IActionResult Index()
        {
            return View(); 
        }
    }
}
