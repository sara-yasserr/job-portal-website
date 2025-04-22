using System.Security.Claims;
using Job_Portal_Project.Models;
using Job_Portal_Project.Services;
using Microsoft.AspNetCore.Mvc;

namespace Job_Portal_Project.Controllers
{
    public class JobApplicationController : Controller
    {
        private JobApplicationService jobApplicationService;
        private string userId;
        public JobApplicationController(JobApplicationService _jobApplicationService)
        {
            jobApplicationService = _jobApplicationService;
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        public IActionResult Index()
        {
           List<JobApplication> UserJobApplications = jobApplicationService.GetUserApplications(userId);

           return View(UserJobApplications);
        }

        public IActionResult Add(int jobId)
        {
            JobApplication jobApplication = new JobApplication{ JobId = jobId};
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveAdd(JobApplication jobApplication)
        {
            if(ModelState.IsValid)
            {
                jobApplication.ApplicantId = userId;
                jobApplicationService.Insert(jobApplication);
                return RedirectToAction("Index", "JobApplication");
            }

            return View("Add",jobApplication);
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

            return View("Edit",EditedJobApplication);
        }
    }
}
