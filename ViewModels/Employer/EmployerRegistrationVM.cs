﻿using System.ComponentModel.DataAnnotations;

namespace job_board.ViewModels.Employer
{
    public class EmployerRegistrationVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

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
