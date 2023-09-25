using System.ComponentModel.DataAnnotations;

namespace job_board.Models
{
    public class Skill
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public Candidate Candidate { get; set; }
    }
}
