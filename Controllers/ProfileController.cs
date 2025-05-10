using System.Security.Claims;
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
        private readonly SignInManager<ApplicationUser> signInManager;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            IProfileService profileService,
          IUserMappingService userMapping,
            SignInManager<ApplicationUser> signInManager
        )
        {
            _userManager = userManager;
            _profileService = profileService;
            this.userMapping = userMapping;
            this.signInManager = signInManager;
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
                ResumePath = user?.ResumePath
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
                ResumePath = user?.ResumePath
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(ProfileViewModel user, string id, IFormFile? ProfilePicture)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Find the user by ID instead of using GetUserAsync which might be using cached data
                    var existingUser = await _userManager.FindByIdAsync(user.Id);
                    if (existingUser == null)
                    {
                        ModelState.AddModelError("", "User not found.");
                        return View("Edit", user);
                    }


                    existingUser.FirstName = user.FirstName;
                    existingUser.LastName = user.LastName;
                    existingUser.Title = user.Title;

                    if (existingUser.Email != user.Email)
                    {

                        var userWithSameEmail = await _userManager.FindByEmailAsync(user.Email);
                        if (userWithSameEmail != null && userWithSameEmail.Id != existingUser.Id)
                        {
                            ModelState.AddModelError("Email", "Email is already taken.");
                            return View("Edit", user);
                        }
                        existingUser.Email = user.Email;

                        existingUser.NormalizedEmail = user.Email.ToUpper();
                    }
                    existingUser.PhoneNumber = user.PhoneNumber;
                    existingUser.City = user.City;
                    existingUser.Country = user.Country;


                    if (ProfilePicture != null && ProfilePicture.Length > 0)
                    {
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var extension = Path.GetExtension(ProfilePicture.FileName).ToLowerInvariant();

                        if (!allowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("ProfilePicture", "Only image files (.jpg, .jpeg, .png, .gif) are allowed.");
                            return View("Edit", user);
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

                    var result = await _userManager.UpdateAsync(existingUser);

                    if (result.Succeeded)
                    {
                        System.Diagnostics.Debug.WriteLine($"User {existingUser.Id} updated successfully");

                        if (existingUser.ProfilePicturePath != null)
                        {
                            var existingClaim = User.FindFirst("ProfilePicturePath");
                            if (existingClaim != null)
                            {
                                await _userManager.RemoveClaimAsync(existingUser, existingClaim);
                            }

                            string imagePath = $"/Images/{existingUser.ProfilePicturePath}";
                            await _userManager.AddClaimAsync(existingUser, new Claim("ProfilePicturePath", imagePath));
                        }

                        await signInManager.RefreshSignInAsync(existingUser);

                        TempData["SuccessMessage"] = "Profile updated successfully!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error updating user: {error.Description}");
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                System.Diagnostics.Debug.WriteLine($"Exception updating user: {ex.Message}");
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
            }

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