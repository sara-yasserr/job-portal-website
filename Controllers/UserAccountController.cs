using Job_Portal_Project.Models;
using Job_Portal_Project.Services;
using Job_Portal_Project.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Job_Portal_Project.Controllers.Account
{
    [Authorize]
    public class UserAccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;
        private readonly IBookmarkService _bookmarkService;

        public UserAccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUserService userService,
            IBookmarkService bookmarkService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
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
            var applications = user.JobApplications?.Count ?? 0;

            var viewModel = new ManageAccountViewModel
            {
                Profile = new ProfileViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Title = user.Title,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    City = user.City,
                    Country = user.Country,
                    Role = user.Role,
                    ProfilePicturePath = user.ProfilePicturePath,
                    ResumePath = user.ResumePath
                },
                NotificationCount = 5, // Placeholder - implement notifications if needed
                BookmarkedJobsCount = bookmarkedJobs.Count,
                ApplicationsCount = applications
            };

            return View(viewModel);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userService.ChangePasswordAsync(user.Id, model.CurrentPassword, model.NewPassword);
            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Error changing password. Please check your current password.");
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["SuccessMessage"] = "Your password has been changed successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}