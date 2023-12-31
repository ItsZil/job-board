﻿using job_board.Models;
using job_board.Utilities;
using job_board.ViewModels.Application;
using job_board.ViewModels.Candidate;
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
        [Authorize]
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

            var userId = AuthHelper.GetUserId(User);
            var isCandidate = User.IsInRole("candidate");
            var isCompany = User.IsInRole("company");

            if (isCompany && User.FindFirstValue(ClaimTypes.NameIdentifier) != companyId.ToString())
            {
                return Forbid();
            }

            if (!_dbHelper.DoesCompanyAdExist(companyId, adId))
            {
                return Forbid();
            }

            var applicationsQuery = _context.Applications
                .Where(a => a.Ad.Id == adId && a.Ad.Company.Id == companyId)
                .AsNoTracking();

            if (isCandidate)
            {
                applicationsQuery = applicationsQuery.Where(a => a.Candidate.Id == userId);
            }

            var applicationResponses = applicationsQuery
                .Select(app => new ApplicationResponseVM
                {
                    Id = app.Id,
                    AdId = app.Ad.Id,
                    ApplicationDate = app.ApplicationDate,
                    CoverLetter = app.CoverLetter,
                    PhoneNumber = app.Candidate.Phone
                })
                .AsNoTracking()
                .ToList();

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
                .Include(a => a.Ad)
                .ThenInclude(a => a.Company)
                .AsSplitQuery()
                .AsNoTracking()
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

            var applicationsResponse = new
            {
                app.Id,
                app.ApplicationDate,
                app.CoverLetter
            };
            return Ok(applicationsResponse);
        }

        // POST: api/companies/{companyId}/ads/{adId}/applications
        [HttpPost]
        [Authorize(Roles = "admin,candidate")]
        public async Task<IActionResult> CreateApplication(int companyId, int adId, [FromBody] CoverLetterVM candidateApp)
        {
            if (candidateApp == null)
            {
                return BadRequest();
            }

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

            if (User.IsInRole("admin") && candidate == null)
            {
                return NotFound("Candidate not found.");
            }
            
            if (candidate == null && !User.IsInRole("admin"))
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
            
            _context.Applications.Add(candidateApplication);
            await _context.SaveChangesAsync();

            var response = new {
                candidateApplication.Id,
                candidateApplication.CoverLetter,
                candidateApplication.ApplicationDate
            };
            return Created(string.Empty, response);
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
                .Include(a => a.Candidate)
                .Include(a => a.Ad)
                .ThenInclude(a => a.Company)
                .AsSplitQuery()
                .FirstOrDefault(a => a.Id == appId);

            int userId = AuthHelper.GetUserId(User);
            if (!User.IsInRole("admin") && userId != app.Candidate.Id)
            {
                return Forbid();
            }

            app.CoverLetter = candidateApp.CoverLetter;
            _context.Applications.Update(app);
            await _context.SaveChangesAsync();

            var updateResponse = new {
                app.Id,
                app.CoverLetter,
                app.ApplicationDate,
            };
            return Ok(updateResponse);
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

            _context.Applications.Remove(app);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
