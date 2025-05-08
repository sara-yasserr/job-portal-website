using Job_Portal_Project.Models;
using Job_Portal_Project.Repositories;
using Job_Portal_Project.Repositories.ApplicationUserRepository;
using Job_Portal_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Job_Portal_Project.ViewModels;

namespace Job_Portal_Project.Controllers
{
    [Authorize(Roles = "Employer")]
    public class EmployerController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserMappingService userMapping;
        private readonly IApplicationUserRepository applicationUser;
        private readonly ICompanyRepository companyRepository;

        public EmployerController(UserManager<ApplicationUser> userManager, IUserMappingService userMapping, IApplicationUserRepository applicationUser, ICompanyRepository companyRepository)
        {
            this.userManager = userManager;
            this.userMapping = userMapping;
            this.applicationUser = applicationUser;
            this.companyRepository = companyRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region Profile
        public async Task<IActionResult> Profile()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        public async Task<IActionResult> Update(string id)
        {
            var user = await applicationUser.GetByIdAsync(id);
            return View("Update", user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveUpdate(ApplicationUser user, string id, IFormFile ProfilePicture)
        {
            if (ModelState.IsValid == true)
            {
                var existingUser = await applicationUser.GetByIdAsync(id);

                if (existingUser == null)
                {
                    ModelState.AddModelError("", "User not found.");
                    return View("Update", user);
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


                await userManager.UpdateAsync(existingUser);
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }
            ModelState.AddModelError("", "Error");
            return View("Update", user);
        }
        #endregion

        #region Company
        public IActionResult AllCompany(string name, string country, string city, int page = 1)
        {

            int pageSize = 5;

            var filteredCompanies = companyRepository.FilterCompanies(name, city, country);

            var totalCompanies = filteredCompanies.Count();
            var totalPages = (int)Math.Ceiling(totalCompanies / (double)pageSize);

            var companies = filteredCompanies
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new CompanyListViewModel
            {
                Companies = companies,
                CurrentPage = page,
                TotalPages = totalPages,
                NameFilter = name,
                CountryFilter = country,
                CityFilter = city
            };

            return View(viewModel);
        }

        public IActionResult AddCompany()
        {
            return View("AddCompany");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveNew(Company company, IFormFile LogoFile)
        {

            if (ModelState.IsValid == true)
            {

                var exsitingCompany = companyRepository.GetAll().FirstOrDefault(c => c.Name.Trim() == company.Name);

                if (exsitingCompany != null)
                {
                    ModelState.AddModelError("Name", "A company with this name already exists.");
                    return View("AddCompany", company);
                }

                if (LogoFile != null && LogoFile.Length > 0)
                {
                    try
                    {
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var extension = Path.GetExtension(LogoFile.FileName).ToLowerInvariant();

                        if (!allowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("LogoPath", "Only image files (.jpg, .jpeg, .png, .gif) are allowed.");
                            return View(company);
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
                            await LogoFile.CopyToAsync(stream);
                        }

                        company.LogoPath = fileName;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("LogoPath", $"Error uploading file: {ex.Message}");
                        return View(company);
                    }
                }

                companyRepository.Insert(company);
                companyRepository.Save();
                TempData["SuccessMessage"] = "Company added successfully!";
                return RedirectToAction("AllCompany");
            }
            return RedirectToAction("AddCompany", company);
        }
        #endregion
    }
}
