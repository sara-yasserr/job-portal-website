﻿using Job_Portal_Project.Models;
using Job_Portal_Project.ViewModels;

namespace Job_Portal_Project.Repositories
{
    public interface IJobApplicationRepository : IRepository<JobApplication>
    {
        public List<JobApplication> GetUserApplications(string userId);
        public List<JobApplication> jobApplications(int jobId);

        public Task<JobApplicantDetailsForApplicantViewModel> ApplicantDetailsAsync(string applicantId);
        public Task<List<JobApplicantDetailsForApplicantViewModel>> AllApplicantApplied(int jobId, string fullName);

        public JobApplication GetJobApplicationByApplicantId(string applicantId);
        public JobApplication GetByJobIdAndApplicantId(int jobId, string applicantId);

    }
}
