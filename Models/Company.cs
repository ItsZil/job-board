using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace job_board.Models
{
    public class Company
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
        [StringLength(100)]
        public string CompanyName { get; set; }

        public string? CompanyDescription { get; set; }

        [Required]
        [StringLength(50)]
        public string Industry { get; set; }

        [StringLength(255)]
        public string? Website { get; set; }
    }
}
