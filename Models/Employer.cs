using System.ComponentModel.DataAnnotations;


namespace job_board.Models
{
    public class Employer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string Company { get; set; }

        public string CompanyDescription { get; set; }

        [Required]
        [StringLength(50)]
        public string Industry { get; set; }

        [StringLength(255)]
        public string Website { get; set; }
    }
}
