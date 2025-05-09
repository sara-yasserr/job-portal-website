using System.Diagnostics;
using Job_Portal_Project.Models;
using Job_Portal_Project.Services.Contracts;
using Job_Portal_Project.ViewModels;
using Job_Portal_Project.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult Index()
        {
            var categories = _jobSearchService.GetAllCategories().Result;
            var model = new HomeViewModel
            {
                Categories = categories
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

        public IActionResult About()
        {
            return View();
        }
    }
}
