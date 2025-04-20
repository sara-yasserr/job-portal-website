using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_Project.Models
{
    public class JobCategory
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<Job> Jobs { get; set; }
    }
}
