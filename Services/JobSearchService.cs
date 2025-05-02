// Services/JobSearchService.cs
using Job_Portal_Project.Models;
using Job_Portal_Project.Models.DbContext;
using Job_Portal_Project.Repositories;
using Job_Portal_Project.Repositories.ApplicationUserRepository;
using Job_Portal_Project.Services.Contracts;
using Job_Portal_Project.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Job_Portal_Project.Services
{
    public class JobSearchService : IJobSearchService
    {
        private readonly JobPortalContext _context;
        private readonly IApplicationUserRepository _userRepository;
        private readonly IJobCategoryRepository _jobCategoryRepository;
        private readonly IJobRepository _jobRepository;

        public JobSearchService(JobPortalContext context, IApplicationUserRepository userRepository,
            IJobCategoryRepository jobCategoryRepository, IJobRepository jobRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _jobCategoryRepository = jobCategoryRepository;
            _jobRepository = jobRepository;   
        }

        public async Task<JobSearchViewModel> SearchJobs(JobSearchViewModel model)
        {
            var query = BuildBaseQuery();
            query = ApplySearchFilters(query, model);
            await ExecuteSearch(query, model);
            await LoadFilterOptions(model);
            return model;
        }

        private IQueryable<Job> BuildBaseQuery()
        {
            return _context.Jobs
                .Include(j => j.Company)
                .Include(j => j.JobCategory)
                .Where(j => j.IsActive && (j.DateClosed == null || j.DateClosed > DateTime.Now));
        }

        private IQueryable<Job> ApplySearchFilters(IQueryable<Job> query, JobSearchViewModel model)
        {
            if (!string.IsNullOrEmpty(model.Keyword))
            {
                query = query.Where(j => j.Title.Contains(model.Keyword) ||
                                     j.Description.Contains(model.Keyword) ||
                                     j.Company.Name.Contains(model.Keyword));
            }

            if (model.CategoryId.HasValue && model.CategoryId > 0)
            {
                query = query.Where(j => j.JobCategoryId == model.CategoryId);
            }

            if (!string.IsNullOrEmpty(model.Location))
            {
                query = query.Where(j => j.City.Contains(model.Location) ||
                                      j.Country.Contains(model.Location));
            }

            if (!string.IsNullOrEmpty(model.EmploymentType))
            {
                query = query.Where(j => j.EmploymentType == model.EmploymentType);
            }

            if (!string.IsNullOrEmpty(model.WorkMode))
            {
                query = query.Where(j => j.WorkMode == model.WorkMode);
            }

            if (!string.IsNullOrEmpty(model.ExperienceLevel))
            {
                query = query.Where(j => j.ExperienceLevel == model.ExperienceLevel);
            }

            return query;
        }

        private async Task ExecuteSearch(IQueryable<Job> query, JobSearchViewModel model)
        {
            // TotalJobs
            model.TotalJobs = await query.CountAsync();

            // orederig jobs
            model.Jobs = await query
                .OrderByDescending(j => j.DatePosted)
                .Skip((model.CurrentPage - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToListAsync();
        }

        private async Task LoadFilterOptions(JobSearchViewModel model)
        {
            // JOB categories laoding
            model.Categories = await _context.JobCategories.ToListAsync();

            // distinct cities loading
            model.Locations = await _context.Jobs
                .Select(j => j.City)
                .Distinct()
                .ToListAsync();

            // employment types loading
            model.EmploymentTypes = await _context.Jobs
                .Select(j => j.EmploymentType)
                .Distinct()
                .ToListAsync();

            // work modes loading
            model.WorkModes = await _context.Jobs
                .Select(j => j.WorkMode)
                .Distinct()
                .ToListAsync();

            // ExperienceLevels 
            model.ExperienceLevels = await _context.Jobs
                .Select(j => j.ExperienceLevel)
                .Distinct()
                .ToListAsync();

            // user loading
            if (!string.IsNullOrEmpty(model.CurrentUserId))
            {
                model.CurrentUser = await _userRepository.GetByIdAsync(model.CurrentUserId);
            }
        }

        public async Task<List<Job>> GetRecentJobs(int count = 6)
        {
            return await _context.Jobs
                .Include(j => j.Company)
                .Include(j => j.JobCategory)
                .Where(j => j.IsActive && (j.DateClosed == null || j.DateClosed > DateTime.Now))
                .OrderByDescending(j => j.DatePosted)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Job> GetJobDetails(int jobId)
        {
            var job = await _context.Jobs
                .Include(j => j.Company)
                .Include(j => j.JobCategory)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null)
            {
                throw new InvalidOperationException($"Job with ID {jobId} not found.");
            }

            return job;
        }

        public async Task<ApplicationUser?> GetCurrentUser(string userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<bool> HasUserApplied(string userId, int jobId)
        {
            return await _context.JobApplications
                .AnyAsync(ja => ja.ApplicantId == userId && ja.JobId == jobId);
        }
       

        public async Task<List<Job>> GetRelatedJobs(int categoryId, int excludeJobId)
        {
            return await _context.Jobs
                .Include(j => j.Company)
                .Where(j => j.JobCategoryId == categoryId &&
                           j.Id != excludeJobId &&
                           j.IsActive &&
                           (j.DateClosed == null || j.DateClosed > DateTime.Now))
                .OrderByDescending(j => j.DatePosted)
                .Take(4)
                .ToListAsync();
        }

        public Task<bool> ToggleFavoriteJob(string userId, int jobId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ApplyForJob(string userId, int jobId)
        {
            throw new NotImplementedException();
        //Categoriiies



        public async Task<List<CategoryWithJobCount>> GetAllCategories()
        {
            // Get all categories and active jobs asynchronously
            var categories =  _jobCategoryRepository.GetAll();
            var activeJobs = ( _jobRepository.GetAll()).Where(j => j.IsActive).ToList();

            // Create the view model
            var result = categories.Select(category => new CategoryWithJobCount
            {
                Category = category,
                JobCount = activeJobs.Count(j => j.JobCategoryId == category.Id)
            })
            .OrderByDescending(x => x.JobCount)
            .ToList();

            return result;
        }
    }
}