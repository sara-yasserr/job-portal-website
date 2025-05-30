﻿
using Job_Portal_Project.Models;
using Job_Portal_Project.Services.Contracts;
using Job_Portal_Project.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Job_Portal_Project.Controllers
{
    [AllowAnonymous]
    public class JobSearchController : Controller
    {
        private readonly IJobSearchService _jobSearchService;

        public JobSearchController(IJobSearchService jobSearchService)
        {
            _jobSearchService = jobSearchService;
        }

        // Display main search page >>>>finnnnnshhhhed ==>:)
        [HttpGet]
        public async Task<IActionResult> Index(JobSearchViewModel model)
        {
            model.CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var results = await _jobSearchService.SearchJobs(model);

            if (!string.IsNullOrEmpty(model.Keyword) && !results.Jobs.Any())
            {
                results.Jobs = await _jobSearchService.GetRecentJobs();
                ViewBag.NoResultsMessage = "No matching jobs found. Here are some recent listings:";
            }

            return View("Index", results);
        }

        // AJAX search
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(JobSearchViewModel model)
        {
            model.CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var results = await _jobSearchService.SearchJobs(model);
            return PartialView("_JobListPartial", results.Jobs);
        }



        // this in Authenticated ...>>>>Finisheddddd
        public async Task<IActionResult> Details(int id)
        {
            var job = await _jobSearchService.GetJobDetails(id);
            if (job == null)
            {
                TempData["ErrorMessage"] = "The requested job doesn't exist or has been closed";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new JobDetailsViewModel
            {
                Job = job,
                RelatedJobs = await _jobSearchService.GetRelatedJobs(job.JobCategoryId, id)
            };

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                viewModel.CurrentUser = await _jobSearchService.GetCurrentUser(userId);
                viewModel.HasApplied = await _jobSearchService.HasUserApplied(userId, id);

            }

            return View("Details", viewModel);
        }

        // Display all categories
        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var categoriesWithCounts = await _jobSearchService.GetAllCategories();
            return View("Categories", categoriesWithCounts);
        }



        // Browse jobs by category
        [HttpGet]
        public async Task<IActionResult> CategoryJobs(int id, string name)
        {
            var model = new JobSearchViewModel
            {
                CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CategoryId = id,
                CategoryName = name
            };

            var results = await _jobSearchService.SearchJobs(model);

            ViewBag.CategoryName = name;
            ViewBag.CategoryId = id;

            if (!results.Jobs.Any())
            {
                ViewBag.NoResultsMessage = $"No jobs found in {name} category.";
            }

            return View(results);
        }

        // Show recent jobs
        [HttpGet]
        public async Task<IActionResult> RecentJobs()
        {
            var recentJobs = await _jobSearchService.GetRecentJobs();
            return View("Index", new JobSearchViewModel { Jobs = recentJobs });
        }

        // Add/remove from favorites
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite(int jobId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "User is not authenticated.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _jobSearchService.ToggleFavoriteJob(userId, jobId);

            if (result)
                TempData["SuccessMessage"] = "Favorites updated successfully";
            else
                TempData["ErrorMessage"] = "Error updating favorites";

            return RedirectToAction(nameof(Details), new { id = jobId });
        }


    }
}