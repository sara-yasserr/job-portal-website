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
        private readonly IUserMappingService userMapping;
     

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            IProfileService profileService,
          IUserMappingService userMapping
        )
        {
            _userManager = userManager;
            _profileService = profileService;
            this.userMapping = userMapping;
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
                ProfilePicturePath = user.ProfilePicturePath,
                ResumePath = user.ResumePath
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
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
                ProfilePicturePath = user.ProfilePicturePath,
                ResumePath = user.ResumePath
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(ApplicationUser user, string id, IFormFile ProfilePicture)
        {
            if (ModelState.IsValid == true)
            {
                var existingUser = await _userManager.GetUserAsync(User);
                

                if (existingUser == null)
                {
                    ModelState.AddModelError("", "User not found.");
                    return View("Edit", user);
                }


                if (ProfilePicture != null && ProfilePicture.Length > 0)
                {

                    try
                    {

                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var extension = Path.GetExtension(ProfilePicture.FileName).ToLowerInvariant();

                        if (!allowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("ProfilePicture", "Only image files (.jpg, .jpeg, .png, .gif) are allowed.");
                            return View("Update", user);
                        }


                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }


                        var fileName = Guid.NewGuid().ToString() + extension;
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ProfilePicture.CopyToAsync(stream);
                        }

                        existingUser.ProfilePicturePath = fileName;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("ProfilePicture", $"Error uploading file: {ex.Message}");
                        return View("Update", user);
                    }
                }
                userMapping.MapToUpdateUser(existingUser, user);


                await _userManager.UpdateAsync(existingUser);
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Error");
            return View("Edit", user);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteResume()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(user.ResumePath))
            {
                return RedirectToAction(nameof(Edit));
            }

            var result = await _profileService.DeleteResumeAsync(user.Id, user.ResumePath);
            if (result)
            {
                TempData["SuccessMessage"] = "Resume deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete resume.";
            }

            return RedirectToAction(nameof(Edit));
        }
    }
}