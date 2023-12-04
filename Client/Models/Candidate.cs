using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace job_board.Models
{
    public class Candidate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        [JsonIgnore]
        public string Password { get; set; }

        [Required]
        [StringLength(100)]
        [JsonIgnore]
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
        public ICollection<Skill> Skills { get; set; } = new List<Skill>();
        public ICollection<Education> Education { get; set; } = new List<Education>();
        public ICollection<JobHistory> JobHistory { get; set; } = new List<JobHistory>();
    }
}
