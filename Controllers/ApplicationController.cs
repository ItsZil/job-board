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
    [Route("api/companies/{companyId}/ads/{adId}/applications")]
    public class ApplicationController : ControllerBase
    {
        private readonly ILogger<ApplicationController> _logger;
        private readonly AppDbContext _context;
        private readonly DbHelper _dbHelper;

        public ApplicationController(ILogger<ApplicationController> logger, AppDbContext context, DbHelper dbHelper)
        {
            _logger = logger;
            _context = context;
            _dbHelper = dbHelper;
        }

        /*
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
        }*/

        // GET: api/companies/{companyId}/ads/{adId}/applications
        [HttpGet]
        [Authorize(Roles = "admin,company")]
        public IActionResult GetAllApplications(int companyId, int adId)
        {
            if (!_dbHelper.DoesCompanyExist(companyId))
            {
                return NotFound("Company not found!");
            }

            if (!_dbHelper.DoesAdExist(adId))
            {
                return NotFound("Ad not found!");
            }

            var applications = _context.Applications
                .Include(a => a.Candidate)
                .Where(a => a.Ad.Company.Id == companyId && a.Ad.Id == adId)
                .AsSplitQuery()
                .ToList();

            if (User.IsInRole("Company") && User.FindFirstValue(ClaimTypes.NameIdentifier) != companyId.ToString())
            {
                return Unauthorized();
            }

            return Ok(applications);
        }

        // GET: api/companies/{companyId}/ads/{adId}/applications/{appId}
        [HttpGet]
        [Route("{appId}")]
        [Authorize]
        public IActionResult GetApplicationById(int companyId, int adId, int appId)
        {
            if (!_dbHelper.DoesCompanyExist(companyId))
            {
                return NotFound("Company not found!");
            }

            if (!_dbHelper.DoesAdExist(adId))
            {
                return NotFound("Ad not found!");
            }
            
            var application = _context.Applications
                .Include(a => a.Candidate)
                .Where(a => a.Ad.Company.Id == companyId && a.Ad.Id == adId)
                .AsSplitQuery()
                .FirstOrDefault(a => a.Id == appId);
            
            if (application == null)
            {
                return NotFound();
            }
            
            int userId = AuthHelper.GetUserId(User);
            switch (User.FindFirstValue(ClaimTypes.Role))
            {
                case "Candidate":
                    if (userId != application.Candidate.Id)
                    {
                        return Unauthorized();
                    }
                    break;
                case "Company":
                    if (userId != application.Ad.Company.Id)
                    {
                        return Unauthorized();
                    }
                    break;
                default:
                    break;
            }

            var applicationsResponse = new Application
            {
                Id = application.Id,
                Ad = new Ad
                {
                    Id = application.Ad.Id,
                    Title = application.Ad.Title,
                    Company = new Company { CompanyName = application.Ad.Company.CompanyName }
                },
                ApplicationDate = application.ApplicationDate
            };
            return Ok(applicationsResponse);
        }

        /*
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
        }*/
    }
}