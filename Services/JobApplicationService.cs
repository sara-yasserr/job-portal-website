using Job_Portal_Project.Models;
using Job_Portal_Project.Repositories;
using Job_Portal_Project.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_Project.Services
{
    public class JobApplicationService : IJobApplicationService
    {
       private readonly IJobApplicationRepository jobApplicationRebo;

       public JobApplicationService( IJobApplicationRepository _jobApplicationRebo)
       {
            jobApplicationRebo = _jobApplicationRebo;
       }

       public List<JobApplication> GetUserApplications(string userId)
       {
            return jobApplicationRebo.GetUserApplications(userId);
        }

       public void Insert(JobApplicationViewModel entity)
       {
            entity.ApplicationDate = DateTime.Now;

            JobApplication application = new JobApplication()
            {
                JobId = entity.JobId,
                ApplicantId = entity.ApplicantId,
                ApplicationDate = entity.ApplicationDate,
                Status = entity.Status,
                SpecificResumePath = entity.ResumePath,
                Applicant = entity.Applicant,
                Job = entity.job
            };
            jobApplicationRebo.Insert(application);
            jobApplicationRebo.Save();
       }

        public void Insert(Job job , ApplicationUser user)
        {
            JobApplication application = new JobApplication()
            {
                JobId = job.Id,
                ApplicantId = user.Id,
                ApplicationDate = DateTime.Now,
                SpecificResumePath = user.ResumePath,
                Applicant = user,
                Job = job
            };
            jobApplicationRebo.Insert(application);
            jobApplicationRebo.Save();
        }
       public JobApplication GetJobApplication(int id)
       {
            return jobApplicationRebo.GetById<int>(id);
       }

       public void Delete<T>(T Id)
       {
            jobApplicationRebo.Delete(Id);
            jobApplicationRebo.Save();

        }

        public void Update(JobApplicationViewModel entity)
        {
            var jobApp = jobApplicationRebo.GetByJobIdAndApplicantId(entity.JobId, entity.ApplicantId);
            jobApp.JobId = entity.JobId;
            jobApp.ApplicantId = entity.ApplicantId;
            jobApp.SpecificResumePath = entity.ResumePath;

            jobApplicationRebo.Update(jobApp);
            jobApplicationRebo.Save();
        }


    }
}
