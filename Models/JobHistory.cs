using System.ComponentModel.DataAnnotations;

namespace job_board.Models
{
    public class JobHistory
    {
        [Key]
        public int Id { get; set; }
        public string Employer { get; set; }
        public string Position { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Candidate Candidate { get; set; }
    }
}
