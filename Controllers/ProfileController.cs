using Job_Portal_Project.Models;
using Job_Portal_Project.Services;
using Job_Portal_Project.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Job_Portal_Project.Controllers.Profile
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProfileService _profileService;
        private readonly IUserService _userService;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            IProfileService profileService,
            IUserService userService)
        {
            _userManager = userManager;
            _profileService = profileService;
            _userService = userService;
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
                Title = user.Title,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                City = user.City,
                Country = user.Country,
                ProfilePicturePath = user.ProfilePicturePath
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileViewModel model)
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

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Title = model.Title;
            user.PhoneNumber = model.PhoneNumber;
            user.City = model.City;
            user.Country = model.Country;

            if (model.ProfilePicture != null)
            {
                await _profileService.UploadProfilePictureAsync(user.Id, model.ProfilePicture);
            }

            await _userService.UpdateUserAsync(user);
            TempData["SuccessMessage"] = "Your profile has been updated successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}