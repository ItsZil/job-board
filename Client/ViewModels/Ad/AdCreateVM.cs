using System.ComponentModel.DataAnnotations;

namespace job_board.ViewModels.Ad
{
    public class AdCreateVM
    {
        [Required]
        [StringLength(75)]
        public string Title { get; set; }

        [Required]
        [StringLength(3000)]
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
