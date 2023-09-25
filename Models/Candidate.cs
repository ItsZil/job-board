using System.ComponentModel.DataAnnotations;

namespace job_board.Models
{
    public class Candidate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        [Required]
        [StringLength(100)]
        public string Salt { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [StringLength(20)]
        public string Phone { get; set; }

        [Required]
        [StringLength(20)]
        public string City { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public ICollection<Skill> Skills { get; set; }
        public ICollection<Education> Education { get; set; }
        public ICollection<JobHistory> JobHistory { get; set; }
    }
}
