using Job_Portal_Project.Models;
using Job_Portal_Project.Repositories;
using Job_Portal_Project.Services;
using Job_Portal_Project.Services.Contracts;
using Job_Portal_Project.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Job_Portal_Project.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_Project.Controllers
{
    public class JobsController : Controller
    {
        private readonly IJobService _jobService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICompanyRepository _companyRepository;
        private readonly IJobCategoryRepository _jobCategoryRepository;


        public JobsController(IJobService jobService, UserManager<ApplicationUser>? userManager, ICompanyRepository companyRepository, IJobCategoryRepository jobCategoryRepository)
        {
            _jobService = jobService;
            _userManager = userManager;
            _companyRepository = companyRepository;
            _jobCategoryRepository = jobCategoryRepository;
        }


        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> IndexAsync(string? title, string? companyName, int? categoryId, int page = 1)
        {
          

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var jobsQuery = _jobService.GetAllJobs()
                                .Where(j => j.EmployerId == user.Id);

            if (!string.IsNullOrWhiteSpace(title))
            {
                title = title.Trim();
                jobsQuery = jobsQuery.Where(j => j.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(companyName))
            {
                companyName = companyName.Trim();
                jobsQuery = jobsQuery.Where(j => j.Company != null && j.Company.Name.Contains(companyName, StringComparison.OrdinalIgnoreCase));
            }


            if (categoryId.HasValue)
            {
                jobsQuery = jobsQuery.Where(j => j.JobCategoryId == categoryId.Value);
            }


            var totalJobs =  jobsQuery.Count();  
            int PageSize = totalJobs < 3 ? totalJobs : 3;
            var totalPages = (int)Math.Ceiling(totalJobs / (double)PageSize);


            var jobs = jobsQuery
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            var model = new JobFilterViewModel
            {
                Jobs = jobs,
                Title = title,
                CompanyName = companyName,
                CategoryId = categoryId,
                Categories = _jobCategoryRepository.GetAll(),
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(model);
        }
















        //[Authorize(Roles = "Employer")]
        //public async Task<IActionResult> IndexAsync(string? title, string? companyName, int? categoryId)
        //{

        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null)
        //    {
        //        return Unauthorized(); 
        //    }

        //    //var jobs = _jobService.GetAllJobs();

        //    var jobs = _jobService.GetAllJobs()
        //                 .Where(j => j.EmployerId == user.Id)
        //                 .ToList();


        //    if (!string.IsNullOrWhiteSpace(title))
        //    {
        //        title = title.Trim();   

        //        jobs = jobs.Where(j => j.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
        //    }


        //    if (!string.IsNullOrWhiteSpace(companyName))
        //    {
        //        companyName = companyName.Trim();
        //        jobs = jobs.Where(j => j.Company != null && j.Company.Name.Contains(companyName, StringComparison.OrdinalIgnoreCase)).ToList();
        //    }


        //    if (categoryId.HasValue)
        //    {
        //        jobs = jobs.Where(j => j.JobCategoryId == categoryId.Value).ToList();
        //    }


        //    var model = new JobFilterViewModel
        //    {
        //        Jobs = jobs,
        //        Title = title,
        //        CompanyName = companyName,
        //        CategoryId = categoryId,
        //        Categories = _jobCategoryRepository.GetAll()
        //    };

        //    return View(model);
        //}





        public ActionResult Details(int id)
        {
            Job job = _jobService.GetJobById(id);

            if (job == null)
            {
                return NotFound();
            }
            return View(job);
        }
        [Authorize(Roles = "Employer")]
        public IActionResult Create()
        {
            ViewBag.Companies = _companyRepository.GetAll();
            ViewBag.Categories = _jobCategoryRepository.GetAll();
            return View();
        }


        [HttpPost]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Create(JobCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                var job = new Job
                {
                    Title = model.Title,
                    Description = model.Description,
                    EmploymentType = model.EmploymentType,
                    WorkMode = model.WorkMode,
                    City = model.City,
                    Country = model.Country,
                    ExperienceLevel = model.ExperienceLevel,
                    VacancyCount = model.VacancyCount,
                    JobCategoryId = model.JobCategoryId,
                    CompanyId = model.CompanyId,
                    DatePosted = DateTime.Now,
                    EmployerId = user.Id
                };

                _jobService.CreateJob(job);
                return RedirectToAction("Index");
            }

            ViewBag.Companies = _companyRepository.GetAll();
            ViewBag.Categories = _jobCategoryRepository.GetAll();
            return View(model);
        }



        [Authorize(Roles = "Employer")]
        public IActionResult Edit(int id)
        {
            var job = _jobService.GetJobById(id);
            if (job == null)
            {
                return NotFound();
            }

            var model = new JobCreateViewModel
            {
                Title = job.Title,
                Description = job.Description,
                EmploymentType = job.EmploymentType,
                WorkMode = job.WorkMode,
                City = job.City,
                Country = job.Country,
                ExperienceLevel = job.ExperienceLevel,
                VacancyCount = job.VacancyCount,
                JobCategoryId = job.JobCategoryId,
                CompanyId = job.CompanyId
            };

            ViewBag.Companies = _companyRepository.GetAll();
            ViewBag.Categories = _jobCategoryRepository.GetAll();

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Edit(int id, JobCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var job = _jobService.GetJobById(id);
                if (job == null)
                {
                    return NotFound();
                }

                job.Title = model.Title;
                job.Description = model.Description;
                job.EmploymentType = model.EmploymentType;
                job.WorkMode = model.WorkMode;
                job.City = model.City;
                job.Country = model.Country;
                job.ExperienceLevel = model.ExperienceLevel;
                job.VacancyCount = model.VacancyCount;
                job.JobCategoryId = model.JobCategoryId;
                job.CompanyId = model.CompanyId;


                _jobService.UpdateJob(job);
                return RedirectToAction("Index");
            }

            ViewBag.Companies = _companyRepository.GetAll();
            ViewBag.Categories = _jobCategoryRepository.GetAll();

            return View(model);
        }

        [Authorize(Roles = "Employer")]
        public IActionResult Delete(int id)
        {
            var job = _jobService.GetJobById(id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Employer")]
        public IActionResult DeleteConfirmed(int id)
        {
            _jobService.DeleteJob(id);
            return RedirectToAction("Index");
        }
    }
}
