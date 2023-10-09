using job_board.Models;
using job_board.Utilities;
using job_board.ViewModels;
using job_board.ViewModels.Application;
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

        // GET: api/companies/{companyId}/ads/{adId}/applications
        [HttpGet]
        [Authorize(Roles = "admin,company")]
        public IActionResult GetAllApplications(int companyId, int adId)
        {
            if (!_dbHelper.DoesCompanyExist(companyId))
            {
                return NotFound("Company not found.");
            }

            if (!_dbHelper.DoesAdExist(adId))
            {
                return NotFound("Ad not found.");
            }

            var applications = _context.Applications
                .Include(a => a.Candidate)
                .Where(a => a.Ad.Company.Id == companyId && a.Ad.Id == adId)
                .AsSplitQuery()
                .ToList();

            if (User.IsInRole("company") && User.FindFirstValue(ClaimTypes.NameIdentifier) != companyId.ToString())
            {
                return Forbid();
            }

            List<ApplicationResponseVM> applicationResponses = new List<ApplicationResponseVM>();
            foreach (var app in applications)
            {
                var applicationsResponse = new ApplicationResponseVM
                {
                    Id = app.Id,
                    ApplicationDate = app.ApplicationDate,
                    CoverLetter = app.CoverLetter,
                    CandidateId = app.Candidate.Id,
                    CandidateFullName = app.Candidate.FirstName + " " + app.Candidate.LastName,
                };
                applicationResponses.Add(applicationsResponse);
            }
            return Ok(applicationResponses);
        }

        // GET: api/companies/{companyId}/ads/{adId}/applications/{appId}
        [HttpGet]
        [Route("{appId}")]
        [Authorize]
        public IActionResult GetApplicationById(int companyId, int adId, int appId)
        {
            if (!_dbHelper.DoesCompanyExist(companyId))
            {
                return NotFound("Company not found.");
            }

            if (!_dbHelper.DoesAdExist(adId))
            {
                return NotFound("Ad not found.");
            }

            if (!_dbHelper.DoesCompanyAdExist(companyId, adId))
            {
                return NotFound("Ad not found for this company.");
            }

            var app = _context.Applications
                .Include(a => a.Candidate)
                .Where(a => a.Ad.Company.Id == companyId && a.Ad.Id == adId)
                .AsSplitQuery()
                .FirstOrDefault(a => a.Id == appId);
            
            if (app == null)
            {
                return NotFound("Application not found.");
            }
            
            int userId = AuthHelper.GetUserId(User);
            if (!User.IsInRole("admin") && userId != app.Candidate.Id && userId != app.Ad.Company.Id)
            {
                return Forbid();
            }

            var applicationsResponse = new ApplicationResponseVM
            {
                Id = app.Id,
                ApplicationDate = app.ApplicationDate,
                CoverLetter = app.CoverLetter,
                CandidateId = app.Candidate.Id,
                CandidateFullName = app.Candidate.FirstName + " " + app.Candidate.LastName,
            };
            return Ok(applicationsResponse);
        }

        // POST: api/companies/{companyId}/ads/{adId}/applications
        [HttpPost]
        [Authorize(Roles = "admin,candidate")]
        public async Task<IActionResult> CreateApplication(int companyId, int adId, [FromBody] CoverLetterVM candidateApp)
        {
            if (!_dbHelper.DoesCompanyExist(companyId))
            {
                return NotFound("Company not found.");
            }

            if (!_dbHelper.DoesAdExist(adId))
            {
                return NotFound("Ad not found.");
            }

            if (!_dbHelper.DoesCompanyAdExist(companyId, adId))
            {
                return NotFound("Ad not found for this company.");
            }
            
            var ad = _context.Ads
                .FirstOrDefault(a => a.Id == adId);

            int userId = AuthHelper.GetUserId(User);
            var candidate = _context.Candidates
                .FirstOrDefault(c => c.Id == userId);
            
            if (candidate == null && User.IsInRole("admin"))
            {
                userId = _context.Candidates
                    .Select(c => c.Id)
                    .FirstOrDefault();
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
                return Created(string.Empty, candidateApplication);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/companies/{companyId}/ads/{adId}/applications/{appId}
        [HttpPut]
        [Route("{appId}")]
        [Authorize(Roles = "admin,candidate")]
        public async Task<IActionResult> UpdateApplication(int companyId, int adId, int appId, [FromBody] CoverLetterVM candidateApp)
        {
            if (!_dbHelper.DoesCompanyExist(companyId))
            {
                return NotFound("Company not found.");
            }

            if (!_dbHelper.DoesAdExist(adId))
            {
                return NotFound("Ad not found.");
            }

            if (!_dbHelper.DoesCompanyAdExist(companyId, adId))
            {
                return NotFound("Ad not found for this company.");
            }

            var app = _context.Applications
                .Where(a => a.Id == appId && a.Ad.Company.Id == companyId && a.Ad.Id == adId)
                .Include(a => a.Candidate)
                .FirstOrDefault();
            
            if (app == null)
            {
                return NotFound("Application not found.");
            }

            int userId = AuthHelper.GetUserId(User);
            if (!User.IsInRole("admin") && userId != app.Candidate.Id)
            {
                return Forbid();
            }

            app.CoverLetter = candidateApp.CoverLetter;
            try
            {
                _context.Applications.Update(app);
                await _context.SaveChangesAsync();
                return Ok(app);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DEL: api/companies/{companyId}/ads/{adId}/applications/{appId}
        [HttpDelete]
        [Route("{appId}")]
        [Authorize(Roles = "admin,candidate")]
        public async Task<IActionResult> DeleteApplication(int companyId, int adId, int appId)
        {
            if (!_dbHelper.DoesCompanyExist(companyId))
            {
                return NotFound("Company not found.");
            }

            if (!_dbHelper.DoesAdExist(adId))
            {
                return NotFound("Ad not found.");
            }

            if (!_dbHelper.DoesCompanyAdExist(companyId, adId))
            {
                return NotFound("Ad not found for this company.");
            }

            var app = _context.Applications
                .Where(a => a.Id == appId && a.Ad.Company.Id == companyId && a.Ad.Id == adId)
                .Include(a => a.Candidate)
                .FirstOrDefault();

            if (app == null)
            {
                return NotFound("Application not found.");
            }
            
            int userId = AuthHelper.GetUserId(User);
            if (!User.IsInRole("admin") && userId != app.Candidate.Id)
            {
                return Forbid();
            }

            try
            {
                _context.Applications.Remove(app);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}