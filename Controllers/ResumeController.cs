using Job_Portal_Project.Models;
using Job_Portal_Project.Services;
using Job_Portal_Project.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Job_Portal_Project.Controllers.Profile
{
    [Authorize]
    public class ResumeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProfileService _profileService;

        public ResumeController(
            UserManager<ApplicationUser> userManager,
            IProfileService profileService)
        {
            _userManager = userManager;
            _profileService = profileService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new ProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Title = user.Title,
                PhoneNumber = user.PhoneNumber,
                City = user.City,
                Country = user.Country,
                ResumePath = user.ResumePath
            };

            return View("Index",viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile resume)
        {
            if (resume == null)
            {
                ModelState.AddModelError(string.Empty, "No resume file selected");
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            await _profileService.UploadResumeAsync(user.Id, resume);
            TempData["SuccessMessage"] = "Your resume has been uploaded successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(user.ResumePath))
            {
                TempData["ErrorMessage"] = "No resume found to delete.";
                return RedirectToAction(nameof(Index));
            }

            await _profileService.DeleteResumeAsync(user.Id, user.ResumePath);
            TempData["SuccessMessage"] = "Your resume has been deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
