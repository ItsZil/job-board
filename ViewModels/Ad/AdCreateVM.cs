using System.ComponentModel.DataAnnotations;

namespace job_board.ViewModels.Ad
{
    public class AdCreateVM
    {
        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        [StringLength(10)]
        public string SalaryFrom { get; set; }

        [Required]
        [StringLength(10)]
        public string SalaryTo { get; set; }

        [Required]
        [StringLength(100)]
        public string Location { get; set; }
    }
}
