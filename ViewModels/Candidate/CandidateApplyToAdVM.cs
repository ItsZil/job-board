using System.ComponentModel.DataAnnotations;

namespace job_board.ViewModels.Candidate
{
    public class CandidateApplyToAdVM
    {
        [Required]
        public int AdId { get; set; }

        [StringLength(1000)]
        public string? CoverLetter { get; set; }
    }
}
