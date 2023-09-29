﻿namespace job_board.Models
{
    public class Education
    {
        public int Id { get; set; }
        public string Institution { get; set; }
        public string Degree { get; set; }
        public DateTime GraduationDate { get; set; }
        public Candidate Candidate { get; set; }
    }
}
