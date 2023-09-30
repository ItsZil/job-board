using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace job_board.Models
{
    public class Application
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [ForeignKey("CandidateId")]
        public Candidate Candidate { get; set; }

        [Required]
        [ForeignKey("AdId")]
        public Ad Ad { get; set; }
        
        public string? CoverLetter { get; set; }
        
        public DateTime ApplicationDate { get; set; } = DateTime.Now;
    }
}
