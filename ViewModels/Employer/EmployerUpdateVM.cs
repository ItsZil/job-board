using System.ComponentModel.DataAnnotations;

namespace job_board.ViewModels.Employer
{
    public class EmployerUpdateVM
    {
        [Required]
        [StringLength(100)]
        public string Company { get; set; }

        public string? CompanyDescription { get; set; }

        [Required]
        [StringLength(50)]
        public string Industry { get; set; }

        [StringLength(255)]
        public string? Website { get; set; }
    }
}
