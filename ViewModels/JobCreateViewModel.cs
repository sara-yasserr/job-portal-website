using System.ComponentModel.DataAnnotations;

namespace Job_Portal_Project.ViewModels
{
    public class JobCreateViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string EmploymentType { get; set; }

        [Required]
        public string WorkMode { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string ExperienceLevel { get; set; }

        [Required]
        public int VacancyCount { get; set; }

        [Required]
        public int JobCategoryId { get; set; }

        [Required]
        public int CompanyId { get; set; }
    }
}

