using job_board.Models;
using job_board.Utilities;
using job_board.ViewModels;
using job_board.ViewModels.Candidate;
using job_board.ViewModels.CandidateVM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

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
        public async Task<IActionResult> RegistrateCandidate([FromBody] CandidateRegistrationVM registrationData)
        {
            byte[] passwordPlain = Encoding.UTF8.GetBytes(registrationData.Password);
            byte[] passwordSalt = RandomNumberGenerator.GetBytes(32);

            string saltString, passwordHashString;
            using (var pbkdf2 = new Rfc2898DeriveBytes(passwordPlain, passwordSalt, 10000, HashAlgorithmName.SHA512))
            {
                byte[] hash = pbkdf2.GetBytes(32);
                saltString = Convert.ToBase64String(passwordSalt);
                passwordHashString = Convert.ToBase64String(hash);
            }

            var candidate = new Candidate
            {
                Email = registrationData.Email,
                FirstName = registrationData.FirstName,
                LastName = registrationData.LastName,
                Phone = registrationData.Phone,
                City = registrationData.City,
                DateOfBirth = registrationData.DateOfBirth != null ? registrationData.DateOfBirth : (DateTime?)null,
                Salt = saltString,
                Password = passwordHashString
            };

            try
            {
                _context.Candidates.Add(candidate);
                await _context.SaveChangesAsync();
                return Ok();
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

            byte[] passwordPlain = Encoding.UTF8.GetBytes(loginData.Password);
            byte[] passwordSalt = Convert.FromBase64String(candidate.Salt);

            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(passwordPlain, passwordSalt, 10000, HashAlgorithmName.SHA512))
            {
                byte[] computedHash = rfc2898DeriveBytes.GetBytes(32);
                if (computedHash.SequenceEqual(Convert.FromBase64String(candidate.Password)))
                {
                    return Ok("Authentication successful.");
                }
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
        public IActionResult GetCandidateApplications(int id)
        {
            var candidate = _context.Candidates.FirstOrDefault(c => c.Id == id);

            if (candidate == null)
            {
                return NotFound();
            }

            int userId = 1; // TODO: get from auth
            if (false) // userId != id
            {
                return Unauthorized();
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
        public async Task<IActionResult> SaveCandidateSkills([FromBody] CandidateSaveSkillsVM saveSkills)
        {
            int id = 1; // TODO: retrieve from auth
            var candidate = await _context.Candidates
                .Include(c => c.Skills)
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (candidate == null)
            {
                return Unauthorized("Not logged in!");
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
        public async Task<IActionResult> SaveCandidateEducation([FromBody] CandidateSaveEducationVM saveEducation)
        {
            int id = 1; // TODO: retrieve from auth
            var candidate = await _context.Candidates
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (candidate == null)
            {
                return Unauthorized("Not logged in!");
            }

            try
            {
                foreach (var education in saveEducation.EducationsToAdd)
                {
                    if (!candidate.Education.Any(e => e.Institution == education.Institution && e.Degree == education.Degree && e.GraduationDate == education.GraduationDate))
                    {
                        candidate.Education.Add(new Education { Institution = education.Institution, Degree = education.Degree, GraduationDate = education.GraduationDate, Candidate = candidate });
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
        public async Task<IActionResult> SaveCandidateJobHistory([FromBody] CandidateSaveJobHistoryVM saveJobHistory)
        {
            int id = 1; // TODO: retrieve from auth
            var candidate = await _context.Candidates
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (candidate == null)
            {
                return NotFound();
            }

            try
            {
                foreach (var jobHistory in saveJobHistory.JobHistoryToAdd)
                {
                    if (!candidate.JobHistory.Any(j => j.Employer == jobHistory.Employer && j.Position == jobHistory.Position
                        && j.StartDate == jobHistory.StartDate && j.EndDate == jobHistory.EndDate || (jobHistory.EndDate == null && j.EndDate == null)))
                    {
                        candidate.JobHistory.Add(new JobHistory { Employer = jobHistory.Employer, Position = jobHistory.Position, StartDate = jobHistory.StartDate, EndDate = jobHistory.EndDate, Candidate = candidate });
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
        public async Task<IActionResult> CandidateApplyToAd([FromBody] CandidateApplyToAdVM candidateApp)
        {
            int candidateId = 1; // TODO: retrieve from auth
            var candidate = _context.Candidates.FirstOrDefault(c => c.Id == candidateId);
            
            if (candidate == null)
            {
                return Unauthorized("Not logged in!");
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