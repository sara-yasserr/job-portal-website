using Job_Portal_Project.Models;
using Job_Portal_Project.Services;
using Job_Portal_Project.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Job_Portal_Project.Controllers.Jobs
{
    [Authorize]
    public class BookmarksController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBookmarkService _bookmarkService;

        public BookmarksController(
            UserManager<ApplicationUser> userManager,
            IBookmarkService bookmarkService)
        {
            _userManager = userManager;
            _bookmarkService = bookmarkService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var bookmarkedJobs = await _bookmarkService.GetBookmarkedJobsAsync(user.Id);

            return View("Index",new BookmarkedJobsViewModel
            {
                BookmarkedJobs = bookmarkedJobs
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int jobId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            await _bookmarkService.RemoveBookmarkAsync(user.Id, jobId);
            TempData["SuccessMessage"] = "Job has been removed from bookmarks.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int jobId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            await _bookmarkService.BookmarkJobAsync(user.Id, jobId);
            TempData["SuccessMessage"] = "Job has been added to bookmarks.";
            return RedirectToAction("Details", "JobSearch", new { id = jobId });
        }
    }
}