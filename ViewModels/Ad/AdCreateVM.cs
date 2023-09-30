﻿using job_board.Models;
using System.ComponentModel.DataAnnotations;

namespace job_board.ViewModels.Ad
{
    public class AdCreateVM
    {
        [Key]
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
        public Models.Employer Employer { get; set; }
    }
}