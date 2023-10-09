using job_board.Models;
using job_board.Utilities;
using job_board.ViewModels.Company;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                return NotFound("No companies found.");
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
                return NotFound("Company not found.");
            }
            
            return Ok(company);
        }

        // POST: api/companies
        [HttpPost]
        [Authorize(Roles = "company,admin")]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyCreationVM companyData)
        {
            if (companyData == null)
            {
                return BadRequest();
            }
            var hashedPasswordAndSalt = AuthHelper.HashPassword(companyData.Password);

            var company = new Company
            {
                Email = companyData.Email,
                CompanyName = companyData.CompanyName,
                CompanyDescription = companyData.CompanyDescription,
                Industry = companyData.Industry,
                Website = companyData.Website,
                Salt = hashedPasswordAndSalt.salt,
                Password = hashedPasswordAndSalt.hashedPassword
            };

            try
            {
                _context.Companies.Add(company);
                await _context.SaveChangesAsync();

                return Created(string.Empty, _context.Companies.Find(company.Id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/companies/{companyId}
        [HttpPut("{companyId}")]
        [Authorize(Roles = "company,admin")]
        public async Task<IActionResult> UpdateCompany(int companyId, [FromBody] CompanyUpdateVM companyData)
        {
            int userId = AuthHelper.GetUserId(User);
            if (userId == 0)
            {
                return Unauthorized();
            }
            var company = _context.Companies.FirstOrDefault(e => e.Id == companyId);

            if (company == null)
            {
                return NotFound("Company not found.");
            }

            if (company.Id != userId && !User.IsInRole("admin"))
            {
                return Forbid();
            }

            company.CompanyName = companyData.Company;
            company.CompanyDescription = companyData.CompanyDescription;
            company.Industry = companyData.Industry;
            company.Website = companyData.Website;

            _context.Companies.Update(company);
            await _context.SaveChangesAsync();

            /*var response = new
            {
                companyId,
                companyData
            };*/
            return Ok(company);
        }

        // DELETE: api/companies/{companyId}
        [HttpDelete("{companyId}")]
        [Authorize(Roles = "company,admin")]
        public async Task<IActionResult> DeleteCompany(int companyId)
        {
            int userId = AuthHelper.GetUserId(User);
            if (userId == 0)
            {
                return Unauthorized();
            }
            var company = _context.Companies.FirstOrDefault(e => e.Id == companyId);

            if (company == null)
            {
                return NotFound("Company not found.");
            }

            if (company.Id != userId && !User.IsInRole("admin"))
            {
                return Forbid();
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}