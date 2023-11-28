using System.ComponentModel.DataAnnotations;

namespace job_board.ViewModels.Company
{
    public class CompanyUpdateVM
    {
        [Required]
        [StringLength(75)]
        public string Company { get; set; }

        [Required]
        [StringLength(1000)]
        public string? CompanyDescription { get; set; }

        [Required]
        [StringLength(50)]
        public string Industry { get; set; }

        [StringLength(50)]
        public string? Website { get; set; }
    }
}
