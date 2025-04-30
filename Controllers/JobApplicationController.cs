//using System.Security.Claims;
//using Job_Portal_Project.Models;
//using Job_Portal_Project.Models.DbContext;
//using Job_Portal_Project.Repositories;
//using Job_Portal_Project.Services;
//using Job_Portal_Project.ViewModels;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace Job_Portal_Project.Controllers
//{
//    [Authorize(Roles = "JobSeeker")]
//    public class JobApplicationController : Controller
//    {
//        private readonly IJobApplicationService jobApplicationService;
//        private readonly UserManager<ApplicationUser> userManager;
//        private readonly IJobRepository jobRepo;
//        IJobApplicationRepository jobAppRepo;
//        public JobApplicationController(IJobApplicationRepository _jobAppRepo,IJobApplicationService _jobApplicationService,UserManager<ApplicationUser> _userManager, IJobRepository _jobRepo)
//        {
//            jobApplicationService = _jobApplicationService;
//            userManager = _userManager;
//            jobRepo = _jobRepo;
//            jobAppRepo = _jobAppRepo;
//        }
//        #region Index
//        public IActionResult Index(int page = 1)
//        {
//            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            var UserJobApplications = jobApplicationService.GetUserApplications(userId).OrderByDescending(a=>a.ApplicationDate).ToList();
//            //--search --//
//            var searchQuery = HttpContext.Request.Query["searchQuery"].ToString().ToLower();
//            if (!string.IsNullOrEmpty(searchQuery))
//            {
//                UserJobApplications = UserJobApplications.AsQueryable().Include(a => a.Job).Include(a => a.Applicant)
//                    .Where(a => a.Job.Company.Name.ToLower().Contains(searchQuery)
//                    || a.Applicant.FirstName.ToLower().Contains(searchQuery)
//                    || a.Applicant.LastName.ToLower().Contains(searchQuery)
//                    || a.Status.ToLower().Contains(searchQuery)
//                    || a.Job.WorkMode.ToLower().Contains(searchQuery)
//                    ||a.Job.EmploymentType.ToLower().Contains(searchQuery)).ToList();
                    
//            }
//            ViewData["searchQuery"] = searchQuery;

//            //--pagination--//
//            int pageSize = 5;
//            int totalJobApp = UserJobApplications.Count();
//            int NoPages = (int)(Math.Ceiling((double)totalJobApp / pageSize));
//            UserJobApplications = UserJobApplications.Skip((page - 1) * pageSize).Take(pageSize).ToList();
//            ViewBag.CurrentPage = page;
//            ViewBag.TotalPages = NoPages;
//            return View(UserJobApplications);
//        }
//        #endregion

//        #region Apply
//        public async Task<IActionResult> Add(int jobId)
//        {
//            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            var user = await userManager.FindByIdAsync(userId);
//            var job = jobRepo.GetById(jobId);
//            var jobApp = jobAppRepo.GetByJobIdAndApplicantId(jobId,userId);
//            var jobAppVM = new JobApplicationViewModel
//            {
//                JobId = jobId,
//                ApplicantId = userId,
//                ResumePath = user.ResumePath,
//                job = job,
//                Applicant = user
//            };
//            //check if the user applied to this job before
//            if (jobApp != null)
//            {
//                TempData["Applied"] = true;
//                return RedirectToAction("Edit",jobAppVM);
//            }
//            return View(jobAppVM);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> SaveAdd(JobApplicationViewModel jobAppVM)
//        {
//            if (jobAppVM.ResumeFile != null)
//            {
//                jobAppVM.ResumePath = await UploadFile(jobAppVM.ResumeFile);
//            }
//            jobAppVM.Applicant = await userManager.FindByIdAsync(jobAppVM.ApplicantId);
//            jobAppVM.job = jobRepo.GetById(jobAppVM.JobId);

//            if (ModelState.IsValid)
//            {
//                jobApplicationService.Insert(jobAppVM);
//                return RedirectToAction("Index", "JobApplication");
//            }

//            return View("Add",jobAppVM);
//        }

//        public async Task<IActionResult> EasyApply(int jobId)
//        {
//            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            var user = await userManager.FindByIdAsync(userId);
//            var job = jobRepo.GetById(jobId);
//            var jobApp = jobAppRepo.GetByJobIdAndApplicantId(jobId, userId);
//            jobApplicationService.Insert(jobApp);

//            return RedirectToAction("Index");
//        }
//        #endregion

//        #region Details & Delete
//        public IActionResult Details(int jobId)
//        {
//           JobApplication jobApplicaion = jobApplicationService.GetJobApplication(jobId);
//            return View(jobApplicaion);
//        }

//        [HttpPost]
//        public IActionResult Delete(int Id) 
//        {
//            jobApplicationService.Delete(Id);
//           return RedirectToAction("Index");
//        }
//        #endregion

//        #region Edit
//        public async Task<IActionResult> Edit(int jobId)
//        {
//            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            var user = await userManager.FindByIdAsync(userId);
//            var job = jobRepo.GetById(jobId);

//            var jobApp = jobAppRepo.GetByJobIdAndApplicantId(jobId, userId);
//            if (jobApp == null)
//            {
//                return NotFound();
//            }

//            var jobAppVM = new JobApplicationViewModel
//            {
//                JobId = jobId,
//                ApplicantId = userId,
//                job = job,
//                Applicant = user,
//                ResumePath = jobApp.SpecificResumePath,
//                ApplicationDate = jobApp.ApplicationDate
//            };

//            if (TempData["Applied"] != null)
//            {
//                ViewBag.Applied = TempData["Applied"];
//            }

//            return View(jobAppVM);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> SaveEdit(JobApplicationViewModel EditedJobAppVM)
//        {
//            if (EditedJobAppVM.ResumeFile != null)
//            {
//                EditedJobAppVM.ResumePath = await UploadFile(EditedJobAppVM.ResumeFile);
//            }
//            EditedJobAppVM.Applicant = await userManager.FindByIdAsync(EditedJobAppVM.ApplicantId);
//            EditedJobAppVM.job = jobRepo.GetById(EditedJobAppVM.JobId);

//            if (ModelState.IsValid)
//            {
//                jobApplicationService.Update(EditedJobAppVM);
//                return RedirectToAction("Index", "JobApplication");
//            }

//            return View("Edit", EditedJobAppVM);
//        }
//        #endregion

//        #region Handling CV File Function
//        public async Task<string> UploadFile(IFormFile resumeFile)
//        {
//            if (resumeFile != null && resumeFile.Length > 0)
//            {
//                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads");
//                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(resumeFile.FileName);
//                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

//                using (var stream = new FileStream(filePath, FileMode.Create))
//                {
//                    await resumeFile.CopyToAsync(stream);
//                }

//                return "/Uploads/" + uniqueFileName;
//            }

//            return null;
//        }
//        #endregion
//    }
//}