using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace job_board.Models
{
    public class Ad
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

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

        public DateTime PostedDate { get; set; } = DateTime.Now;

        [Required]
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
    }
}
