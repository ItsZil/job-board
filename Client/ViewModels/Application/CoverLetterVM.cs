using System.ComponentModel.DataAnnotations;

namespace job_board.ViewModels.Candidate
{
    public class CoverLetterVM
    {
        [StringLength(1000)]
        public string? CoverLetter { get; set; }
    }
}
