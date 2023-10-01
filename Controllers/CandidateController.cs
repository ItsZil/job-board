using job_board.Models;
using job_board.Utilities;
using job_board.ViewModels;
using job_board.ViewModels.Candidate;
using job_board.ViewModels.CandidateVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace job_board.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CandidateController : ControllerBase
    {
        private readonly ILogger<CandidateController> _logger;
        private readonly AppDbContext _context;

        public CandidateController(ILogger<CandidateController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost("RegisterCandidate")]
        public async Task<IActionResult> RegisterCandidate([FromBody] CandidateRegistrationVM registrationData)
        {
            var hashedPasswordAndSalt = AuthHelper.HashPassword(registrationData.Password);

            var candidate = new Candidate
            {
                Email = registrationData.Email,
                FirstName = registrationData.FirstName,
                LastName = registrationData.LastName,
                Phone = registrationData.Phone,
                City = registrationData.City,
                DateOfBirth = registrationData.DateOfBirth != null ? registrationData.DateOfBirth : (DateTime?)null,
                Salt = hashedPasswordAndSalt.salt,
                Password = hashedPasswordAndSalt.hashedPassword
            };

            try
            {
                _context.Candidates.Add(candidate);
                await _context.SaveChangesAsync();

                int candidateId = await _context.Candidates.Where(c => c.Email == registrationData.Email).Select(c => c.Id).FirstOrDefaultAsync();
                var token = AuthHelper.GenerateJwtToken(candidateId, "Candidate");
                
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("LoginCandidate")]
        public async Task<IActionResult> LoginCandidate([FromBody] LoginVM loginData)
        {
            var candidate = await _context.Candidates.FirstOrDefaultAsync(c => c.Email == loginData.Email);
            if (candidate == null)
            {
                return NotFound("Account not found.");
            }

            if (AuthHelper.DoesPasswordMatch(loginData.Password, candidate.Salt, candidate.Password))
            {
                var token = AuthHelper.GenerateJwtToken(candidate.Id, "Candidate");
                return Ok(new { Token = token });
            }
            return Unauthorized("Invalid password.");
        }

        [HttpGet("GetAllCandidates")]
        public IActionResult GetAllCandidates()
        {
            var candidates = _context.Candidates
                .Include(c => c.Skills)
                .Include(c => c.Education)
                .Include(c => c.JobHistory)
                .AsSplitQuery()
                .ToList();
            if (candidates == null)
            {
                return NotFound();
            }
            return Ok(candidates);
        }

        [HttpGet("GetCandidateById")]
        public IActionResult GetCandidateById(int id)
        {
            var candidate = _context.Candidates
                .Include(c => c.Skills)
                .Include(c => c.Education)
                .Include(c => c.JobHistory)
                .AsSplitQuery()
                .FirstOrDefault(c => c.Id == id);
            
            if (candidate == null)
            {
                return NotFound();
            }
            return Ok(candidate);
        }

        [HttpGet("GetCandidateApplications")]
        [Authorize(Roles = "Candidate")]
        public IActionResult GetCandidateApplications(int id)
        {
            int userId = AuthHelper.GetUserId(User);
            if (userId != id)
            {
                return Unauthorized();
            }
            
            var candidate = _context.Candidates.FirstOrDefault(c => c.Id == id);
            if (candidate == null)
            {
                return NotFound();
            }

            var applicationsResponse = _context.Applications
                .Include(a => a.Ad)
                    .ThenInclude(ad => ad.Employer)
                .Where(app => app.Candidate.Id == id)
                .Select(app => new Application
                {
                    Id = app.Id,
                    Ad = new Ad
                    {
                        Id = app.Ad.Id,
                        Title = app.Ad.Title,
                        Employer = new Employer { Company = app.Ad.Employer.Company }
                    },
                    ApplicationDate = app.ApplicationDate
                })
                .ToList();

            return Ok(applicationsResponse);
        }

        [HttpPost("SaveCandidateSkills")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> SaveCandidateSkills([FromBody] CandidateSaveSkillsVM saveSkills)
        {
            int userId = AuthHelper.GetUserId(User);
            if (userId == 0)
            {
                return Unauthorized();
            }
            
            var candidate = await _context.Candidates
                .Include(c => c.Skills)
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.Id == userId);
            
            if (candidate == null)
            {
                return NotFound("Failed to find user!");
            }

            try
            {
                foreach (var skill in saveSkills.SkillsToAdd)
                {
                    if (!candidate.Skills.Any(s => s.Name == skill.Name))
                    {
                        candidate.Skills.Add(new Skill { Name = skill.Name });
                    }
                }
                await _context.SaveChangesAsync();
                return Ok("Skills saved successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("SaveCandidateEducation")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> SaveCandidateEducation([FromBody] CandidateSaveEducationVM saveEducation)
        {
            int userId = AuthHelper.GetUserId(User);
            if (userId == 0)
            {
                return Unauthorized();
            }
            
            var candidate = await _context.Candidates
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => c.Id == userId);

            if (candidate == null)
            {
                return NotFound("Failed to find user!");
            }

            try
            {
                foreach (var education in saveEducation.EducationsToAdd)
                {
                    if (!candidate.Education.Any(e => e.Institution == education.Institution && e.Degree == education.Degree && e.GraduationDate == education.GraduationDate))
                    {
                        candidate.Education.Add(new Education { Institution = education.Institution, Degree = education.Degree, GraduationDate = education.GraduationDate });
                    }
                }
                await _context.SaveChangesAsync();
                return Ok("Education saved successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("SaveCandidateJobHistory")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> SaveCandidateJobHistory([FromBody] CandidateSaveJobHistoryVM saveJobHistory)
        {
            int userId = AuthHelper.GetUserId(User);
            if (userId == 0)
            {
                return Unauthorized();
            }

            var candidate = await _context.Candidates
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => c.Id == userId);

            if (candidate == null)
            {
                return NotFound("Failed to find user!");
            }

            try
            {
                foreach (var jobHistory in saveJobHistory.JobHistoryToAdd)
                {
                    if (!candidate.JobHistory.Any(j => j.Employer == jobHistory.Employer && j.Position == jobHistory.Position
                        && j.StartDate == jobHistory.StartDate && j.EndDate == jobHistory.EndDate || (jobHistory.EndDate == null && j.EndDate == null)))
                    {
                        candidate.JobHistory.Add(new JobHistory { Employer = jobHistory.Employer, Position = jobHistory.Position, StartDate = jobHistory.StartDate, EndDate = jobHistory.EndDate });
                    }
                }
                await _context.SaveChangesAsync();
                return Ok("Job history saved successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("CandidateApplyToAd")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> CandidateApplyToAd([FromBody] CandidateApplyToAdVM candidateApp)
        {
            int userId = AuthHelper.GetUserId(User);
            if (userId == 0)
            {
                return Unauthorized();
            }

            var candidate = _context.Candidates.FirstOrDefault(c => c.Id == userId);
            
            if (candidate == null)
            {
                return NotFound("Failed to find user!");
            }

            var ad = _context.Ads
                .FirstOrDefault(a => a.Id == candidateApp.AdId);
            if (ad == null)
            {
                return BadRequest();
            }

            var candidateApplication = new Application
            {
                Candidate = candidate,
                Ad = ad,
                CoverLetter = candidateApp.CoverLetter
            };
            try
            {
                _context.Applications.Add(candidateApplication);
                await _context.SaveChangesAsync();
                return Ok("Application to ad saved.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}