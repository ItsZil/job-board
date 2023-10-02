using job_board.Models;
using job_board.Utilities;
using job_board.ViewModels;
using job_board.ViewModels.Company;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace job_board.Controllers
{
    [ApiController]
    [Route("api/companies")]
    public class CompanyController : ControllerBase
    {
        private readonly ILogger<CompanyController> _logger;
        private readonly AppDbContext _context;
        private readonly DbHelper _dbHelper;

        public CompanyController(ILogger<CompanyController> logger, AppDbContext context, DbHelper dbHelper)
        {
            _logger = logger;
            _context = context;
            _dbHelper = dbHelper;
        }

        // GET: api/companies
        [HttpGet]
        public IActionResult GetAllCompanies()
        {
            var employers = _context.Companies
                .ToList();

            if (employers == null)
            {
                return NotFound();
            }
            
            return Ok(employers);
        }

        // GET: api/companies/{companyId}
        [HttpGet]
        [Route("{companyId}")]
        public IActionResult GetCompanyById(int companyId)
        {
            var company = _context.Companies
                .AsSplitQuery()
                .FirstOrDefault(c => c.Id == companyId);
            
            if (company == null)
            {
                return NotFound();
            }
            
            return Ok(company);
        }

        /*

        [HttpPost("GetCompanyAds")]
        public IActionResult GetCompanyAds(int id)
        {
            var employer = _context.Companys.FirstOrDefault(c => c.Id == id);

            if (employer == null)
            {
                return NotFound();
            }

            var ads = _context.Ads
                .Where(a => a.Company.Id == id)
                .ToList();

            return Ok(ads);
        }

        [HttpPost("UpdateCompany")]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> UpdateCompany([FromBody] CompanyUpdateVM employerData)
        {
            int userId = AuthHelper.GetUserId(User);
            if (userId == 0)
            {
                return Unauthorized();
            }
            var employer = _context.Companys.FirstOrDefault(e => e.Id == userId);

            if (employer == null)
            {
                return NotFound();
            }

            if (employer.Id != userId)
            {
                return Unauthorized();
            }

            employer.Company = employerData.Company;
            employer.CompanyDescription = employerData.CompanyDescription;
            employer.Industry = employerData.Industry;
            employer.Website = employerData.Website;

            _context.Companys.Update(employer);
            await _context.SaveChangesAsync();

            return Ok();
        }
        */
    }
}