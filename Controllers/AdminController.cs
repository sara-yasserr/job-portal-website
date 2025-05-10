using Job_Portal_Project.Models;
using Job_Portal_Project.Models.DbContext;
using Job_Portal_Project.Repositories;
using Job_Portal_Project.Repositories.ApplicationUserRepository;
using Job_Portal_Project.Services;
using Job_Portal_Project.ViewModels.Admin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;

namespace Job_Portal_Project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private JobPortalContext _context;
        private ICompanyRepository _companyRepository;
        private IJobRepository _jobRepository;
        private IJobApplicationRepository _jobApplicationRepository;
        private IApplicationUserRepository _applicationUserRepository;
        private IJobCategoryRepository _categoryRepository;
        private IFavouritesRepository _favouritesRepository;
        private IAdminDashboardService _dashboardService;
        private readonly RoleManager<IdentityRole> roleManager;
        private UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(IFavouritesRepository favouritesRepository, ICompanyRepository companyRepository
                    , IJobApplicationRepository jobApplicationRepository, IJobRepository jobRepository
                    , IApplicationUserRepository applicationUserRepository, IJobCategoryRepository jobCategoryRepository
                    , IAdminDashboardService adminDashboardService, UserManager<ApplicationUser> userManager
                    , JobPortalContext context, RoleManager<IdentityRole> roleManager, IWebHostEnvironment webHostEnvironment)
        {
            _favouritesRepository = favouritesRepository;
            _companyRepository = companyRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _jobRepository = jobRepository;
            _applicationUserRepository = applicationUserRepository;
            _categoryRepository = jobCategoryRepository;
            _dashboardService = adminDashboardService;
            _context = context;
            this.roleManager = roleManager;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Dashboard()
        {
            if (!User.IsInRole("Admin"))
            {
                return Unauthorized();
            }
            var viewModel = await _dashboardService.GetDashboardDataAsync();
            return View("Dashboard", viewModel);
        }
        //Jobs
        public async Task<IActionResult> ViewJobs(int? companyId, int? categoryId, int page = 1, int pageSize = 10)
        {
            var viewModel = await _dashboardService.GetDashboardDataAsync();
            var jobs = viewModel.Jobs.AsQueryable();
            if (companyId.HasValue)
            {
                jobs = jobs.Where(j => j.CompanyId == companyId.Value);
            }
            if (categoryId.HasValue)
            {
                jobs = jobs.Where(j => j.JobCategoryId == categoryId.Value);
            }
            var totalJobs = jobs.Count();
            var totalPages = (int)Math.Ceiling(totalJobs / (double)pageSize);
            if (page > totalPages)
                page = totalPages == 0 ? 1 : totalPages;

            var paginatedJobs = jobs
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

            ViewBag.Companies = viewModel.Companies;
            ViewBag.Categories = viewModel.Categories;
            ViewBag.SelectedCompany = companyId;
            ViewBag.SelectedCategory = categoryId;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalJobs / (double)pageSize);

            return View("ViewJobs", paginatedJobs);
        }
        public async Task<IActionResult> DeleteJob(int id)
        {
            var job = _jobRepository.GetById(id);
            //check for job applications
            var jobApplications = _context.JobApplications.Where(j => j.JobId == id).ToList();
            var favourites = _context.Favourites.Where(f => f.JobId == id).ToList();
            if (jobApplications.Count > 0)
            {
                TempData["JobError"] = "Cannot delete job with applications.";
                return RedirectToAction("ViewJobs");
            }
            if (job == null)
            {
                return NotFound();
            }
            //delete from favourites repo
            if (favourites.Count > 0)
            {
                foreach (var item in favourites)
                {
                    _favouritesRepository.Delete(item.Id);
                }
            }
            _jobRepository.Delete(id);
            _jobRepository.Save();
            return RedirectToAction("ViewJobs");
        }
        public IActionResult Applications(string searchTerm, int page = 1, int pageSize = 10)
        {

            var applicationsQuery = _context.JobApplications
             .Include(j => j.Job)
             .ThenInclude(j => j.Company)
             .Include(j => j.Applicant)
             .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                applicationsQuery = applicationsQuery.Where(a =>
                    (a.Applicant.FirstName + " " + a.Applicant.LastName)
                        .ToLower()
                        .Contains(searchTerm.Trim().ToLower()));
            }
            int totalApplications = applicationsQuery.Count();
            int totalPages = (int)Math.Ceiling(totalApplications / (double)pageSize);

            if (page > totalPages)
                page = totalPages == 0 ? 1 : totalPages;

            var applications = applicationsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewData["SearchTerm"] = searchTerm;
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;

            return View("Applications", applications);
        }
        //Delete Application
        public async Task<IActionResult> DeleteApplication(int id)
        {
            var application = _jobApplicationRepository.GetById(id);
            if (application == null)
            {
                return NotFound();
            }
            _jobApplicationRepository.Delete(id);
            _jobApplicationRepository.Save();
            return RedirectToAction("Applications");
        }

        //Users
        public async Task<IActionResult> Users(string? search, string? country, string? city, string? role, int page = 1, int pageSize = 10)
        {
            var users = await _applicationUserRepository.GetAllAsync();
            var query = users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(u =>
                    (!string.IsNullOrEmpty(u.FirstName) && u.FirstName.ToLower().Contains(search.ToLower())) ||
                    (!string.IsNullOrEmpty(u.LastName) && u.LastName.ToLower().Contains(search.ToLower())) ||
                    (!string.IsNullOrEmpty(u.Email) && u.Email.ToLower().Contains(search.ToLower()))
                );

            if (!string.IsNullOrWhiteSpace(country))
                query = query.Where(u => u.Country == country);

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(u => u.City == city);

            if (!string.IsNullOrWhiteSpace(role))
                query = query.Where(u => u.Role == role);

            // Total Count before pagination
            int totalUsers = query.Count();

            // Apply Pagination
            var paginatedUsers = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // ViewBag for filters & pagination
            ViewBag.Countries = users.Select(u => u.Country).Distinct().ToList();
            ViewBag.Cities = users.Select(u => u.City).Distinct().ToList();
            ViewBag.Roles = users.Select(u => u.Role).Distinct().ToList();
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);
            ViewBag.Search = search;
            ViewBag.SelectedCountry = country;
            ViewBag.SelectedCity = city;
            ViewBag.SelectedRole = role;

            return View("Users", paginatedUsers);
        }


        public async Task<IActionResult> UserDetails(string id)
        {
            var user = await _applicationUserRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View("UserDetails", user);
        }

        //Edit User
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _applicationUserRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View("EditUser", user);
        }
        #region Save Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEdit(ApplicationUser model, IFormFile? profilePicture, IFormFile? resumeFile)
        {
            var user = await _applicationUserRepository.GetByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            // Update fields
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.City = model.City;
            user.Country = model.Country;

            if (user.Role == "Employer")
            {
                user.Title = model.Title;
            }

            // Save profile picture if uploaded
            if (profilePicture != null && profilePicture.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/profiles");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{profilePicture.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePicture.CopyToAsync(fileStream);
                }

                user.ProfilePicturePath = $"/uploads/profiles/{uniqueFileName}";
            }

            // Save resume if JobSeeker
            if (user.Role == "JobSeeker" && resumeFile != null && resumeFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/resumes");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{resumeFile.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await resumeFile.CopyToAsync(fileStream);
                }

                user.ResumePath = $"/uploads/resumes/{uniqueFileName}";
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "User updated successfully.";
                return RedirectToAction("Users");
            }

            // If failed
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
        #endregion


        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _applicationUserRepository.GetByIdAsync(id);

            if (user == null)
            {
                throw new Exception();
            }

            var userRole = await _userManager.GetRolesAsync(user);
            if (userRole.FirstOrDefault() == "JobSeeker")
            {
                var jobApplications = _jobApplicationRepository.GetAll().Where(j => j.ApplicantId == id).ToList();
                if (jobApplications.Count > 0)
                {
                    TempData["UserError"] = "Cannot delete Job Seeker with job applications.";
                    return RedirectToAction("Users");
                }
            }
            if (userRole.FirstOrDefault() == "Employer")
            {
                var jobs = _jobRepository.GetAll().Where(j => j.EmployerId == id).ToList();
                if (jobs.Count > 0)
                {
                    TempData["UserError"] = "Cannot delete Employer with job applications.";
                    return RedirectToAction("Users");
                }
            }

            await _applicationUserRepository.DeleteAsync(user);
            return RedirectToAction("Users");
        }

        //Companies
        [HttpGet]
        public async Task<IActionResult> Companies(string searchTerm, string country, int page = 1, int pageSize = 10)
        {
            var companies = _companyRepository.GetAll();

            // Filter by search term
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower().Trim();
                companies = companies.Where(c => c.Name.ToLower().Contains(searchTerm)).ToList();
            }

            // Filter by country
            if (!string.IsNullOrWhiteSpace(country))
            {
                companies = companies.Where(c => c.Country != null && c.Country.ToLower() == country.ToLower()).ToList();
            }

            var totalCompanies = companies.Count();
            var totalPages = (int)Math.Ceiling(totalCompanies / (double)pageSize);

            if (page > totalPages)
                page = totalPages == 0 ? 1 : totalPages;

            var pagedCompanies = companies
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Create the countries list
            var countries = _companyRepository.GetAll()
                .Where(c => !string.IsNullOrEmpty(c.Country))
                .Select(c => c.Country)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            ViewData["SearchTerm"] = searchTerm;
            ViewData["Country"] = country;
            ViewData["Countries"] = countries; // Just List<string>
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;



            return View(pagedCompanies);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCompany(Company company, IFormFile? logoFile)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(company.Name))
                {
                    ModelState.AddModelError("Name", "Company name is required");
                }
                if (string.IsNullOrEmpty(company.Country))
                {
                    ModelState.AddModelError("Country", "Country is required");
                }
                if (string.IsNullOrEmpty(company.City))
                {
                    ModelState.AddModelError("City", "City is required");
                }

                // Explicitly remove any validation errors for logoFile
                if (ModelState.ContainsKey("logoFile"))
                {
                    ModelState.Remove("logoFile");
                }

                if (ModelState.IsValid)
                {
                    // Handle logo upload
                    if (logoFile != null && logoFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/logos");
                        Directory.CreateDirectory(uploadsFolder);
                        var uniqueFileName = $"{Guid.NewGuid()}_{logoFile.FileName}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await logoFile.CopyToAsync(fileStream);
                        }
                        company.LogoPath = $"/uploads/logos/{uniqueFileName}";
                    }
                    else
                    {
                        company.LogoPath = "/default-logo.png";
                    }

                    // Save company to database
                    _companyRepository.Insert(company);
                    _companyRepository.Save();
                    TempData["CompanySuccess"] = "Company created successfully.";
                    return RedirectToAction("Companies");
                }
                else
                {
                    // If validation fails
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    TempData["CompanyError"] = "Please fix the validation errors: " + string.Join(", ", errors);
                    return RedirectToAction("Companies");
                }
            }
            catch (Exception ex)
            {
                TempData["CompanyError"] = "Error creating company: " + ex.Message;
                return RedirectToAction("Companies");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var company = _companyRepository.GetById(id);

            if (company == null)
            {
                TempData["CompanyError"] = "Company not found.";
                return RedirectToAction("Companies");
            }

            // check if the company is linked to any jobs
            var relatedJobs = _jobRepository.GetAll().Any(j => j.CompanyId == id);

            if (relatedJobs)
            {
                TempData["CompanyError"] = "Cannot delete this company because it has related jobs.";
                return RedirectToAction("Companies");
            }

            // safe to delete
            _companyRepository.Delete(id);
            _companyRepository.Save();
            TempData["CompanySuccess"] = "Company deleted successfully.";
            return RedirectToAction("Companies");
        }
        public async Task<IActionResult> EditCompany(int id)
        {
            var company = _companyRepository.GetById(id);
            if (company == null)
            {
                return NotFound();
            }
            return View("EditCompany", company);
        }

        //Edit Company
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCompany(Company model, IFormFile? logoFile)
        {
            if (!ModelState.IsValid)
                return View("EditCompany", model);

            var company = _companyRepository.GetById(model.Id);
            if (company == null)
                return NotFound();

            company.Name = model.Name;
            company.Country = model.Country;
            company.City = model.City;
            company.Description = model.Description;

            if (logoFile != null && logoFile.Length > 0)
            {
                // Save file and update company.LogoPath
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/logos");
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(logoFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await logoFile.CopyToAsync(stream);
                }

                company.LogoPath = fileName;
            }

            _companyRepository.Update(company);
            _companyRepository.Save();
            TempData["CompanySuccess"] = "Company updated successfully!";
            return RedirectToAction("Companies");
        }



        public IActionResult Role()
        {
            var roles = roleManager.Roles.ToList();
            var viewModel = new RoleViewModel
            {
                ExistingRoles = roles
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            if (usersInRole.Any())
            {
                TempData["RoleError"] = $"Cannot delete role '{role.Name}' because it's assigned to one or more users.";
                return RedirectToAction("Role");
            }

            var result = await roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                TempData["RoleError"] = $"Failed to delete role '{role.Name}'.";
            }

            return RedirectToAction("Role");
        }


        [HttpPost]
        public async Task<IActionResult> SaveRole(RoleViewModel roleVM)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole()
                {
                    Name = roleVM.RoleName
                };

                IdentityResult result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Role");
                }
                else
                {

                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("Role", item.Description);
                    }
                }
            }
            return View("Role", roleVM);
        }
        #region Assign Roles
        public async Task<IActionResult> ManageUserRoles(string searchTerm, string roleFilter, int page = 1, int pageSize = 10)
        {
            var users = await _applicationUserRepository.GetAllAsync();
            var roles = roleManager.Roles.Select(r => r.Name).ToList();

            var userRoleViewModels = new List<UserWithRolesViewModel>();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                userRoleViewModels.Add(new UserWithRolesViewModel
                {
                    UserId = user.Id,
                    FullName = user.FirstName + " " + user.LastName,
                    Email = user.Email,
                    Title = user.Title,
                    CurrentRole = userRoles.FirstOrDefault() ?? "None",
                    AllRoles = roles
                });
            }

            // Filter by searchTerm
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower().Trim();
                userRoleViewModels = userRoleViewModels.Where(u =>
                    u.FullName.ToLower().Contains(searchTerm) ||
                    u.Email.ToLower().Contains(searchTerm)
                ).ToList();
            }

            // Filter by Role
            if (!string.IsNullOrWhiteSpace(roleFilter) && roleFilter != "All")
            {
                userRoleViewModels = userRoleViewModels.Where(u => u.CurrentRole == roleFilter).ToList();
            }

            int totalUsers = userRoleViewModels.Count;
            int totalPages = (int)Math.Ceiling((double)totalUsers / pageSize);

            var pagedUsers = userRoleViewModels
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewData["SearchTerm"] = searchTerm;
            ViewData["RoleFilter"] = roleFilter;
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["AllRoles"] = new List<string> { "All" }.Concat(roles).ToList();

            return View("ManageUserRoles", pagedUsers);
        }

        #endregion
        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            var user = await _applicationUserRepository.GetByIdAsync(userId);
            if (user == null)
                return NotFound();

            // Get current roles and remove them
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            // Add the new role
            await _userManager.AddToRoleAsync(user, role);

            // Update the user's Role column manually
            user.Role = role;
            await _applicationUserRepository.UpdateAsync(user); // <--- VERY IMPORTANT!

            return RedirectToAction("ManageUserRoles");
        }

        //view all users Resumes
        public async Task<IActionResult> ViewResumes(string searchTerm, int page = 1, int pageSize = 10)
        {
            var users = await _applicationUserRepository.GetAllAsync();
            var resumes = new List<ResumeViewModel>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("JobSeeker") && !string.IsNullOrEmpty(user.ResumePath))
                {
                    resumes.Add(new ResumeViewModel
                    {
                        UserID = user.Id,
                        FullName = user.FirstName + " " + user.LastName,
                        Email = user.Email,
                        Title = user.Title,
                        ResumePath = user.ResumePath
                    });
                }
            }
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower().Trim();
                resumes = resumes
                    .Where(r => r.FullName.ToLower().Contains(term))
                    .ToList();
            }
            var total = resumes.Count();
            var totalPages = (int)Math.Ceiling(total / (double)pageSize);
            if (page > totalPages)
                page = totalPages == 0 ? 1 : totalPages;

            var pagedResumes = resumes
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewData["SearchTerm"] = searchTerm;
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;

            return View("ViewResumes", pagedResumes);
        }





    }
}
