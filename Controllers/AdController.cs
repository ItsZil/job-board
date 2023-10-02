using job_board.Models;
using job_board.Utilities;
using job_board.ViewModels.Ad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace job_board.Controllers
{
    [Route("api/companies/{companyId}/ads")]
    [ApiController]
    public class AdController : ControllerBase
    {
        private readonly ILogger<AdController> _logger;
        private readonly AppDbContext _context;
        private readonly DbHelper _dbHelper;

        public AdController(ILogger<AdController> logger, AppDbContext context, DbHelper dbHelper)
        {
            _logger = logger;
            _context = context;
            _dbHelper = dbHelper;
        }

        // GET: api/companies/{companyId}/ads
        [HttpGet]
        public IActionResult GetAllAds(int companyId)
        {
            if (!_dbHelper.DoesCompanyExist(companyId))
            {
                return NotFound("Company not found.");
            }

            var ads = _context.Ads
                .Include(a => a.Company)
                .Where(a => a.Company.Id == companyId)
                .ToList();

            if (ads == null)
            {
                return NotFound("Ad not found.");
            }
            return Ok(ads);
        }

        // GET: api/companies/{companyId}/ads/{adId}
        [HttpGet]
        [Route("{adId}")]
        public IActionResult GetAdById(int adId, int companyId)
        {
            if (!_dbHelper.DoesCompanyExist(companyId))
            {
                return NotFound("Company not found.");
            }

            if (!_dbHelper.DoesAdExist(adId))
            {
                return NotFound("Ad not found.");
            }

            var ad = _context.Ads
                .Include(a => a.Company)
                .Where(a => a.Company.Id == companyId)
                .FirstOrDefault(a => a.Id == adId);
            
            if (ad == null)
            {
                return NotFound();
            }

            return Ok(ad);
        }

        // POST: api/companies/{companyId}/ads
        [HttpPost]
        [Authorize(Roles = "company,admin")]
        public async Task<IActionResult> CreateAd(int companyId, [FromBody] AdCreateVM adData)
        {            
            var company = _context.Companies.Find(companyId);
            if (company == null)
            {
                return NotFound("Company not found.");
            }

            int userId = AuthHelper.GetUserId(User);
            if (company.Id != userId && !User.IsInRole("admin"))
            {
                return Forbid();
            }

            var ad = new Ad
            {
                Title = adData.Title,
                Description = adData.Description,
                SalaryFrom = adData.SalaryFrom,
                SalaryTo = adData.SalaryTo,
                Location = adData.Location,
                PostedDate = DateTime.Now,
                Company = company
            };
            _context.Ads.Add(ad);
            await _context.SaveChangesAsync();
            
            return Created(string.Empty, ad);
        }

        // PUT: api/companies/{companyId}/ads/{adId}
        [HttpPut]
        [Authorize(Roles = "company,admin")]
        public async Task<IActionResult> UpdateAd(int companyId, int adId, [FromBody] AdUpdateVM adData)
        {
            if (!_dbHelper.DoesCompanyExist(companyId))
            {
                return NotFound("Company not found.");
            }
            
            var ad = await _context.Ads
                .Include(e => e.Company)
                .FirstOrDefaultAsync(a => a.Id == adId && a.Company.Id == companyId);
            if (ad == null)
            {
                return NotFound("Ad not found.");
            }

            int userId = AuthHelper.GetUserId(User);
            if (ad.Company.Id != userId && !User.IsInRole("admin"))
            {
                return Forbid();
            }

            ad.Title = adData.Title;
            ad.Description = adData.Description;
            ad.SalaryFrom = adData.SalaryFrom;
            ad.SalaryTo = adData.SalaryTo;
            ad.Location = adData.Location;
            ad.PostedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return Created(string.Empty, ad);
        }

        // DELETE: api/companies/{companyId}/ads/{adId}
        [HttpDelete]
        [Authorize(Roles = "admin,company")]
        public async Task<IActionResult> DeleteAd(int companyId, int adId)
        {
            if (!_dbHelper.DoesCompanyExist(companyId))
            {
                return NotFound("Company not found.");
            }
            
            var ad = await _context.Ads
                .Include(e => e.Company)
                .FirstOrDefaultAsync(a => a.Id == adId && a.Company.Id == companyId);
            if (ad == null)
            {
                return NotFound();
            }

            int userId = AuthHelper.GetUserId(User);
            if (ad.Company.Id != userId && !User.IsInRole("admin"))
            {
                return Forbid();
            }

            _context.Ads.Remove(ad);
            await _context.SaveChangesAsync();
            return Ok("Ad deleted successfully.");
        }
    }
}
