using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using Job_Portal_Project.Models;
using Job_Portal_Project.Models.DbContext;
using Job_Portal_Project.Repositories;
using Job_Portal_Project.Services;
using Job_Portal_Project.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Job_Portal_Project.Controllers
{
    public class JobApplicationController : Controller
    {
        #region cstr
        private readonly IJobApplicationService jobApplicationService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IJobRepository jobRepo;
        IJobApplicationRepository jobAppRepo;
        private readonly SmtpSettings _smtpSettings;
        public JobApplicationController(IJobApplicationRepository _jobAppRepo,IJobApplicationService _jobApplicationService,UserManager<ApplicationUser> _userManager, IJobRepository _jobRepo, IOptions<SmtpSettings> _smtpSettings)
        {
            jobApplicationService = _jobApplicationService;
            userManager = _userManager;
            jobRepo = _jobRepo;
            jobAppRepo = _jobAppRepo;
            this._smtpSettings = _smtpSettings.Value;
        }
        #endregion

        #region Index
        public IActionResult Index(int page = 1)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var UserJobApplications = jobApplicationService.GetUserApplications(userId).OrderByDescending(a=>a.ApplicationDate).ToList();
            //--search --//
            var searchQuery = HttpContext.Request.Query["searchQuery"].ToString().ToLower();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                UserJobApplications = UserJobApplications.AsQueryable().Include(a => a.Job).Include(a => a.Applicant)
                    .Where(a => a.Job.Company.Name.ToLower().Contains(searchQuery)
                    || a.Applicant.FirstName.ToLower().Contains(searchQuery)
                    || a.Applicant.LastName.ToLower().Contains(searchQuery)
                    || a.Status.ToLower().Contains(searchQuery)
                    || a.Job.WorkMode.ToLower().Contains(searchQuery)
                    ||a.Job.EmploymentType.ToLower().Contains(searchQuery)).ToList();
                    
            }
            ViewData["searchQuery"] = searchQuery;

            //--pagination--//
            int pageSize = 10;
            int totalJobApp = UserJobApplications.Count();
            int NoPages = (int)(Math.Ceiling((double)totalJobApp / pageSize));
            UserJobApplications = UserJobApplications.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = NoPages;
            return View(UserJobApplications);
        }
        #endregion

        #region Apply
        public async Task<IActionResult> Add(int jobId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId);
            var job = jobRepo.GetById(jobId);
            if (job.IsActive == false)
            {
                return NotFound();
            }
            var jobApp = jobAppRepo.GetByJobIdAndApplicantId(jobId,userId);
            var jobAppVM = new JobApplicationViewModel
            {
                JobId = jobId,
                ApplicantId = userId,
                ResumePath = user.ResumePath,
                job = job,
                Applicant = user
            };
            //check if the user applied to this job before
            if (jobApp != null)
            {
                TempData["Applied"] = true;
                return RedirectToAction("Edit",jobAppVM);
            }
            return View(jobAppVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAdd(JobApplicationViewModel jobAppVM)
        {
            if (jobAppVM.ResumeFile != null)
            {
                jobAppVM.ResumePath = await UploadFile(jobAppVM.ResumeFile);
            }
            jobAppVM.Applicant = await userManager.FindByIdAsync(jobAppVM.ApplicantId);
            jobAppVM.job = jobRepo.GetById(jobAppVM.JobId);

            if (ModelState.IsValid)
            {
                jobApplicationService.Insert(jobAppVM);
                return RedirectToAction("Index", "JobApplication");
            }

            return View("Add",jobAppVM);
        }

        public async Task<IActionResult> EasyApply(int jobId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId);
            var job = jobRepo.GetById(jobId);
            var jobApp = jobAppRepo.GetByJobIdAndApplicantId(jobId, userId);
            if (jobApp != null)
            {
                var jobAppVM = new JobApplicationViewModel
                {
                    JobId = jobId,
                    ApplicantId = userId,
                    ResumePath = user.ResumePath,
                    job = job,
                    Applicant = user
                };
                TempData["Applied"] = true;
                return RedirectToAction("Edit", jobAppVM);
            }
            jobApplicationService.Insert(job,user);

            return RedirectToAction("Index");
        }
        #endregion

        #region Delete

        [HttpPost]
        public IActionResult Delete(int Id) 
        {
            jobApplicationService.Delete(Id);
           return RedirectToAction("Index");
        }
        #endregion

        #region Edit
        public async Task<IActionResult> Edit(int jobId)
        {    
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId);
            var job = jobRepo.GetById(jobId);

            var jobApp = jobAppRepo.GetByJobIdAndApplicantId(jobId, userId);
            if (jobApp == null)
            {
                return NotFound();
            }

            var jobAppVM = new JobApplicationViewModel
            {
                JobId = jobId,
                ApplicantId = userId,
                job = job,
                Applicant = user,
                ResumePath = jobApp.SpecificResumePath,
                ApplicationDate = jobApp.ApplicationDate
            };

            if (TempData["Applied"] != null)
            {
                ViewBag.Applied = TempData["Applied"];
            }

            return View(jobAppVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEdit(JobApplicationViewModel EditedJobAppVM)
        {
            if (EditedJobAppVM.ResumeFile != null)
            {
                EditedJobAppVM.ResumePath = await UploadFile(EditedJobAppVM.ResumeFile);
            }
            EditedJobAppVM.Applicant = await userManager.FindByIdAsync(EditedJobAppVM.ApplicantId);
            EditedJobAppVM.job = jobRepo.GetById(EditedJobAppVM.JobId);

            if (ModelState.IsValid)
            {
                jobApplicationService.Update(EditedJobAppVM);
                return RedirectToAction("Index", "JobApplication");
            }

            return View("Edit", EditedJobAppVM);
        }
        #endregion

        #region Handling CV File Function
        public async Task<string> UploadFile(IFormFile resumeFile)
        {
            if (resumeFile != null && resumeFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads/resumes");
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(resumeFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await resumeFile.CopyToAsync(stream);
                }

                return "/Uploads/resumes/" + uniqueFileName;
            }

            return null;
        }
        #endregion

        #region Make By Ahmed Ali
        public async Task<IActionResult> ShowAllApplicants(int jobId, string fullName)
        {
            var model = await jobAppRepo.AllApplicantApplied(jobId, fullName);
            ViewData["FullName"] = fullName;
            ViewData["JobId"] = jobId;

            return View("ShowAllApplicants", model);
        }
        public async Task<IActionResult> ShowDetialsOfApplicant(string ApplicantId)
        {
            var model = await jobAppRepo.ApplicantDetailsAsync(ApplicantId);

            if (model == null)
            {
                TempData["ErrorMessage"] = "Applicant not found!";
                return RedirectToAction("ShowAllApplicants");
            }

            return View(model);
        }


        public IActionResult ViewResume(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return NotFound();
            }


            if (!Path.HasExtension(path))
            {
                path += ".pdf";
            }

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path.TrimStart('/'));

            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound();
            }

            string contentType = "application/pdf";

            var fileBytes = System.IO.File.ReadAllBytes(fullPath);
            return File(fileBytes, contentType);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeApplicantStatus(string applicantId, string status)
        {
            var applicant = jobAppRepo.GetJobApplicationByApplicantId(applicantId);
            if (applicant == null)
            {
                return NotFound();
            }

            var app = await jobAppRepo.ApplicantDetailsAsync(applicantId);
            var job = jobRepo.GetById(applicant.JobId);

            var oldStatus = applicant.Status;

            if (oldStatus != status)
            {
                applicant.Status = status;
                jobAppRepo.Update(applicant);


                if (oldStatus == "Accept" && status != "Accept")
                {
                    job.VacancyCount += 1;
                }
                else if (oldStatus != "Accept" && status == "Accept")
                {
                    job.VacancyCount -= 1;
                }

                job.IsActive = job.VacancyCount > 0;
                jobRepo.Update(job);

                jobAppRepo.Save();
                jobRepo.Save();


                switch (status)
                {
                    case "Interview Process":
                        SendInterviewProcessEmail(app.Email, app.FirstName, job.Company.Name, job.Title);
                        break;
                    case "Accept":
                        SendAcceptEmail(app.Email, app.FirstName, job.Company.Name, job.Title);
                        break;
                    case "Rejected":
                        SendRejectedEmail(app.Email, app.FirstName, job.Company.Name, job.Title);
                        break;
                }
            }
            ViewData["StatusMessage"] = $"Status for this applicant has been changed to: {status}";
            return RedirectToAction("ShowDetialsOfApplicant", new { ApplicantId = applicantId });
        }

        private void SendEmail(string toEmail, string subject, string body, string displayName)
        {
            try
            {
                var smtpClient = new SmtpClient(_smtpSettings.Host)
                {
                    Port = _smtpSettings.Port,
                    Credentials = new NetworkCredential(_smtpSettings.Email, _smtpSettings.Password),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.Email, displayName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false,
                };

                mailMessage.To.Add(toEmail);

                smtpClient.Send(mailMessage);

                TempData["SuccessMessage"] = "Email has been sent successfully to the applicant.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to send email. Error: {ex.Message}";
            }
        }

        public void SendInterviewProcessEmail(string emailAddress, string applicantName, string companyName, string jobTitle)
        {
            var interviewDate = DateTime.Now.AddDays(3).ToString("dddd, MMMM dd, yyyy 'at' 10:00 AM");
            var subject = $"Interview Process Update - {companyName}";
            var body = $"Dear {applicantName},\n\n" +
                       $"Congratulations! You have moved to the interview stage for the \"{jobTitle}\" position at {companyName}.\n\n" +
                       $"Your interview is scheduled for {interviewDate}.\n\n" +
                       $"Please let us know if you have any questions prior to the interview.\n\n" +
                       $"Best regards,\n" +
                       $"{companyName} Recruitment Team";

            SendEmail(emailAddress, subject, body, companyName);
        }
        public void SendAcceptEmail(string emailAddress, string applicantName, string companyName, string jobTitle)
        {
            var subject = $"Job Offer - {companyName}";
            var body = $"Dear {applicantName},\n\n" +
                       $"We are delighted to offer you the \"{jobTitle}\" position at {companyName}.\n\n" +
                       $"We were impressed by your skills and are excited to welcome you to our team.\n\n" +
                       $"Please reply to this email to confirm your acceptance.\n\n" +
                       $"Best regards,\n" +
                       $"{companyName} Recruitment Team";

            SendEmail(emailAddress, subject, body, companyName);
        }
        public void SendRejectedEmail(string emailAddress, string applicantName, string companyName, string jobTitle)
        {
            var subject = $"Application Update - {companyName}";
            var body = $"Dear {applicantName},\n\n" +
                       $"Thank you for your interest in the \"{jobTitle}\" position at {companyName}.\n\n" +
                       $"After careful review, we regret to inform you that we will not be moving forward with your application at this time.\n\n" +
                       $"We encourage you to apply for future openings that match your profile.\n\n" +
                       $"Best regards,\n" +
                       $"{companyName} Recruitment Team";

            SendEmail(emailAddress, subject, body, companyName);
        }
    }
    #endregion
}
  
