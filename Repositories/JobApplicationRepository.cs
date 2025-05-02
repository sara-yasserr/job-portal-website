using Job_Portal_Project.Models;
using Job_Portal_Project.Models.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_Project.Repositories
{
    public class JobApplicationRepository : IJobApplicationRepository
    {
        private readonly JobPortalContext _context;
        public JobApplicationRepository(JobPortalContext context)
        {
            _context = context;
        }
        public List<JobApplication> GetAll()
        {
            return _context.JobApplications.ToList();
        }
        public JobApplication GetById<I>(I id)
        {
            return _context.JobApplications.Find(id);
        }
        public void Insert(JobApplication entity)
        {
            _context.JobApplications.Add(entity);
        }
        public void Update(JobApplication entity)
        {
            _context.JobApplications.Update(entity);
        }
        public void Delete<I>(I id)
        {
            var jobApplication = _context.JobApplications.Find(id);
            if (jobApplication != null)
            {
                _context.JobApplications.Remove(jobApplication);
            }
        }
        public void Save()
        {
            _context.SaveChanges();
        }
        public int Count()
        {
            return _context.JobApplications.Count();
        }

        public List<JobApplication> GetUserApplications(string userId)
        {
            return _context.JobApplications.Where(a => a.ApplicantId == userId).ToList();
        }

        #region Made By Ahmed Ali
        public List<JobApplication> jobApplications(int jobId)
        {
            return _context.JobApplications.Where(a => a.JobId == jobId).ToList();
        }

        public async Task<JobApplicantDetailsForApplicantViewModel> ApplicantDetailsAsync(string applicantId)
        {
            return await _context.JobApplications
                .Join(_context.Users, app => app.ApplicantId, applicant => applicant.Id, (app, applicant) => new { app, applicant })
                .Where(x => x.app.ApplicantId == applicantId)
                .Select(x => new JobApplicantDetailsForApplicantViewModel
                {
                    FirstName = x.applicant.FirstName,
                    LastName = x.applicant.LastName,
                    Country = x.applicant.Country,
                    City = x.applicant.City,
                    Email = x.applicant.Email,
                    PhoneNumber = x.applicant.PhoneNumber,
                    SpecificResumePath = x.app.SpecificResumePath,
                    JobId = x.app.JobId,
                    ApplicantId = x.app.ApplicantId,
                    status = x.app.Status,

                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<JobApplicantDetailsForApplicantViewModel>> AllApplicantApplied(int jobId, string fullName)
        {
            var query = _context.JobApplications
                            .Join(_context.Users, app => app.ApplicantId, applicant => applicant.Id,
                                  (app, applicant) => new { app, applicant })
                            .Where(x => x.app.JobId == jobId)
                            .Select(x => new JobApplicantDetailsForApplicantViewModel
                            {
                                ApplicantId = x.app.ApplicantId,
                                FirstName = x.applicant.FirstName,
                                LastName = x.applicant.LastName,
                                status = x.app.Status,
                                ApplicationDate = x.app.ApplicationDate,
                                ProfilePicturePath = x.applicant.ProfilePicturePath,
                                JobId = x.app.JobId
                            });
            if (!string.IsNullOrEmpty(fullName))
            {
                fullName = fullName.ToLower();
                query = query.Where(a => (a.FirstName + " " + a.LastName).ToLower().Contains(fullName));
            }
            return await query.ToListAsync();
        }

        public JobApplication GetJobApplicationByApplicantId(string applicantId)
        {
            return _context.JobApplications.FirstOrDefault(x => x.ApplicantId == applicantId);
        }
        #endregion
    }
}
