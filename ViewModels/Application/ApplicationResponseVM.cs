﻿namespace job_board.ViewModels.Application
{
    public class ApplicationResponseVM
    {
        public int Id { get; set; }
        public int AdId { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string? CoverLetter { get; set; }
        public string PhoneNumber { get; set; }
    }
}
