using System.ComponentModel.DataAnnotations;

namespace Job_Portal_Project.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string? LogoPath { get; set; }

        //Navigation Properties
        public virtual List<Job> Jobs { get; set; }
    }
}
