using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using Job_Portal_Project.Models;
using Job_Portal_Project.Repositories;
using Job_Portal_Project.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Job_Portal_Project.Controllers
{
    public class JobApplicationController : Controller
    {
        private IJobApplicationService jobApplicationService;
        private readonly IJobApplicationRepository jobApplicationRepository;
        private readonly IJobRepository jobRepository;
        private readonly SmtpSettings _smtpSettings;
        private string? userId;
        public JobApplicationController(IJobApplicationService _jobApplicationService, IJobApplicationRepository jobApplicationRepository, IOptions<SmtpSettings> _smtpSettings, IJobRepository jobRepository)
        {
            jobApplicationService = _jobApplicationService;
            this.jobApplicationRepository = jobApplicationRepository;
            this.jobRepository = jobRepository;
            this._smtpSettings = _smtpSettings.Value;
        }
        public IActionResult Index()
        {
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<JobApplication> UserJobApplications = jobApplicationService.GetUserApplications(userId);

            return View(UserJobApplications);
        }

        public IActionResult Add(int jobId)
        {
            JobApplication jobApplication = new JobApplication { JobId = jobId };
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveAdd(JobApplication jobApplication)
        {
            if (ModelState.IsValid)
            {
                jobApplication.ApplicantId = userId;
                jobApplicationService.Insert(jobApplication);
                return RedirectToAction("Index", "JobApplication");
            }

            return View("Add", jobApplication);
        }

        public IActionResult Details(int jobId)
        {
            JobApplication jobApplicaion = jobApplicationService.GetJobApplication(jobId);
            return View(jobApplicaion);
        }

        public IActionResult Delete(int jobId)
        {
            jobApplicationService.Delete(jobId);
            return RedirectToAction("Index", "JobApplication");
        }

        public IActionResult Edit(int jobApplicationId)
        {
            JobApplication jobApplication = jobApplicationService.GetJobApplication(jobApplicationId);
            return View(jobApplication);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveEdit(JobApplication EditedJobApplication)
        {
            if (ModelState.IsValid)
            {
                jobApplicationService.Update(EditedJobApplication);
                return RedirectToAction("Index", "JobApplication");
            }

            return View("Edit", EditedJobApplication);
        }

        #region Make By Ahmed Ali
        public async Task<IActionResult> ShowAllApplicants(int jobId, string fullName)
        {
            var model = await jobApplicationRepository.AllApplicantApplied(jobId, fullName);
            ViewData["FullName"] = fullName;
            ViewData["JobId"] = jobId;

            return View("ShowAllApplicants", model);
        }
        public async Task<IActionResult> ShowDetialsOfApplicant(string ApplicantId)
        {
            var model = await jobApplicationRepository.ApplicantDetailsAsync(ApplicantId);

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
            var applicant = jobApplicationRepository.GetJobApplicationByApplicantId(applicantId);
            var app = await jobApplicationRepository.ApplicantDetailsAsync(applicantId);
            var job = jobRepository.GetById(applicant.JobId);

            if (applicant == null)
            {
                return NotFound();
            }

            var applicantStatus = applicant.Status;
            if (applicantStatus != status)
            {
                applicant.Status = status;
                jobApplicationRepository.Update(applicant);
                jobApplicationRepository.Save();

                switch (status)
                {
                    case "Interview Process":
                        SendInterviewProcessEmail(app.Email, app.FirstName, job.Company.Name, job.Title);
                        break;
                    case "Accept":
                        SendAcceptEmail(app.Email, app.FirstName, job.Company.Name, job.Title);
                        job.VacancyCount--;
                        jobRepository.Save();
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