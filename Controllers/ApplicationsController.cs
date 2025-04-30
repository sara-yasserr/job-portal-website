using Job_Portal_Project.Models;
using Job_Portal_Project.Services;
using Job_Portal_Project.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Job_Portal_Project.Controllers.Jobs
{
    [Authorize]
    public class ApplicationsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJobSearchService _jobApplicationService;

        public ApplicationsController(
            UserManager<ApplicationUser> userManager,
            IJobSearchService jobApplicationService)
        {
            _userManager = userManager;
            _jobApplicationService = jobApplicationService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var applications = await _jobApplicationService.GetCurrentUser(user.Id);

            return View(applications);
        }

      

        
    }
}