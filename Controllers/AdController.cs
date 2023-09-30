using job_board.Models;
using job_board.Utilities;
using job_board.ViewModels.Ad;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> CreateAd([FromBody] AdCreateVM adData)
        {
            int employerId = 1; // TODO: get employer id from auth
            var employer = _context.Employers.Find(employerId);
            if (employer == null) // todo: or not authenticated?
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
        public IActionResult GetAdApplicants(int id)
        {
            var ad = _context.Ads
                .Include(a => a.Employer)
                .FirstOrDefault(a => a.Id == id);
            
            if (ad == null)
            {
                return NotFound();
            }

            int userId = 1; // TODO: get user id from auth
            if (false) // ad.Employer.Id != userId
            {
                return Unauthorized();
            }

            var applications = _context.Applications
                .Include(a => a.Candidate)
                .Where(a => a.Ad.Id == id)
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
                }
                CoverLetter = a.CoverLetter,
                ApplicationDate = a.ApplicationDate,
            }).ToList();

            return Ok(adApplications);
        }

        [HttpPost("UpdateAd")]
        public async Task<IActionResult> UpdateAd([FromBody] AdUpdateVM adData)
        {
            var ad = await _context.Ads
                .Include(e => e.Employer)
                .FirstOrDefaultAsync(a => a.Id == adData.Id);
            if (ad == null)
            {
                return NotFound();
            }

            int userId = 1; // todo: get from auth
            if (false) // ad.Employer.Id != userId
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
        public async Task<IActionResult> DeleteAd(int id)
        {
            var ad = await _context.Ads
                .Include(e => e.Employer)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (ad == null)
            {
                return NotFound();
            }

            int userId = 8; // todo: get from auth
            if (false) // ad.Employer.Id != userId
            {
                return Unauthorized();
            }

            _context.Ads.Remove(ad);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
