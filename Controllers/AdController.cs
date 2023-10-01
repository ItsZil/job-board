using job_board.Models;
using job_board.Utilities;
using job_board.ViewModels.Ad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Xml.Linq;

namespace job_board.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdController : ControllerBase
    {
        private readonly ILogger<AdController> _logger;
        private readonly AppDbContext _context;

        public AdController(ILogger<AdController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost("CreateAd")]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> CreateAd([FromBody] AdCreateVM adData)
        {
            int userId = AuthHelper.GetUserId(User);
            if (userId == 0)
            {
                return Unauthorized();
            }
            var employer = _context.Employers.Find(userId);
            if (employer == null)
            {
                return Unauthorized();
            }

            var ad = new Ad
            {
                Title = adData.Title,
                Description = adData.Description,
                SalaryFrom = adData.SalaryFrom,
                SalaryTo = adData.SalaryTo,
                Location = adData.Location,
                PostedDate = DateTime.Now,
                Employer = employer
            };
            _context.Ads.Add(ad);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("GetAllAds")]
        public IActionResult GetAllAds()
        {
            var ads = _context.Ads
                .Include(a => a.Employer)
                .ToList();
            
            if (ads == null)
            {
                return NotFound();
            }
            return Ok(ads);
        }

        [HttpGet("GetAdById")]
        public IActionResult GetAdById(int id)
        {
            var ad = _context.Ads
                .Include(a => a.Employer)
                .FirstOrDefault(a => a.Id == id);
            
            if (ad == null)
            {
                return NotFound();
            }
            return Ok(ad);
        }

        [HttpGet("GetAdApplicants")]
        [Authorize(Roles = "Employer")]
        public IActionResult GetAdApplicants(int id)
        {
            var ad = _context.Ads
                .Include(a => a.Employer)
                .FirstOrDefault(a => a.Id == id);
            
            if (ad == null)
            {
                return NotFound();
            }

            int userId = AuthHelper.GetUserId(User);
            if (userId == 0 || ad.Employer.Id != userId)
            {
                return Unauthorized();
            }

            var applications = _context.Applications
                .Include(a => a.Candidate)
                    .ThenInclude(s => s.Skills)
                .Include(a => a.Candidate)
                    .ThenInclude(c => c.JobHistory)
                .Include(a => a.Candidate)
                    .ThenInclude(c => c.Education)
                .Where(a => a.Ad.Id == id)
                .AsSplitQuery()
                .ToList();

            var adApplications = applications.Select(a => new
            {
                Id = a.Id,
                Candidate = new
                {
                    Id = a.Candidate.Id,
                    Email = a.Candidate.Email,
                    FirstName = a.Candidate.FirstName,
                    LastName = a.Candidate.LastName,
                    Phone = a.Candidate.Phone,
                    City = a.Candidate.City,
                    Skills = a.Candidate.Skills,
                    JobHistory = a.Candidate.JobHistory,
                    Education = a.Candidate.Education
                },
                CoverLetter = a.CoverLetter,
                ApplicationDate = a.ApplicationDate,
            }).ToList();

            return Ok(adApplications);
        }

        [HttpPost("UpdateAd")]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> UpdateAd([FromBody] AdUpdateVM adData)
        {
            var ad = await _context.Ads
                .Include(e => e.Employer)
                .FirstOrDefaultAsync(a => a.Id == adData.Id);
            if (ad == null)
            {
                return NotFound();
            }

            int userId = AuthHelper.GetUserId(User);
            if (userId == 0 || ad.Employer.Id != userId)
            {
                return Unauthorized();
            }

            ad.Title = adData.Title;
            ad.Description = adData.Description;
            ad.SalaryFrom = adData.SalaryFrom;
            ad.SalaryTo = adData.SalaryTo;
            ad.Location = adData.Location;
            ad.PostedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("DeleteAd")]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> DeleteAd(int id)
        {
            var ad = await _context.Ads
                .Include(e => e.Employer)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (ad == null)
            {
                return NotFound();
            }

            int userId = AuthHelper.GetUserId(User);
            if (userId == 0 || ad.Employer.Id != userId)
            {
                return Unauthorized();
            }

            _context.Ads.Remove(ad);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
