using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_Project.Models
{

    public class Favourites
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int JobId { get; set; }
        [Required]
        public string UserId { get; set; }
        public DateTime DateAdded { get; set; }
        [ForeignKey("JobId")]
        public virtual Job Job { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
