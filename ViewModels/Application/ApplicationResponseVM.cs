namespace job_board.ViewModels.Application
{
    public class ApplicationResponseVM
    {
        public int Id { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string? CoverLetter { get; set; }
        public int CandidateId { get; set; }
        public string CandidateFullName { get; set; }
    }
}
