using System.Diagnostics;
using Job_Portal_Project.Models;
using Job_Portal_Project.Services.Contracts;
using Job_Portal_Project.ViewModels;
using Job_Portal_Project.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IJobSearchService _jobSearchService;

        public HomeController(ILogger<HomeController> logger, IJobSearchService jobSearchService)
        {
            _logger = logger;
            _jobSearchService = jobSearchService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _jobSearchService.GetAllCategories();
            var latestJobs = await _jobSearchService.GetRecentJobs(6);

            var model = new HomeViewModel
            {
                Categories = categories,
                LatestJobs = latestJobs
            };
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.Client, NoStore = false)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
