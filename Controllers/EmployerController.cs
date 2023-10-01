using job_board.Models;
using job_board.Utilities;
using job_board.ViewModels;
using job_board.ViewModels.Employer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace job_board.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployerController : ControllerBase
    {
        private readonly ILogger<EmployerController> _logger;
        private readonly AppDbContext _context;

        public EmployerController(ILogger<EmployerController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost("RegisterEmployer")]
        public async Task<IActionResult> RegisterEmployer([FromBody] EmployerRegistrationVM registrationData)
        {
            var hashedPasswordAndSalt = AuthHelper.HashPassword(registrationData.Password);

            var employer = new Employer
            {
                Email = registrationData.Email,
                Company = registrationData.Company,
                CompanyDescription = registrationData.CompanyDescription,
                Industry = registrationData.Industry,
                Website = registrationData.Website,
                Salt = hashedPasswordAndSalt.salt,
                Password = hashedPasswordAndSalt.hashedPassword
            };

            try
            {
                _context.Employers.Add(employer);
                await _context.SaveChangesAsync();

                var token = AuthHelper.GenerateJwtToken(employer.Id, "Employer");
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("LoginEmployer")]
        public async Task<IActionResult> LoginEmployer([FromBody] LoginVM loginData)
        {
            var employer = await _context.Employers.FirstOrDefaultAsync(c => c.Email == loginData.Email);
            if (employer == null)
            {
                return NotFound("Account not found.");
            }

            if (AuthHelper.DoesPasswordMatch(loginData.Password, employer.Salt, employer.Password))
            {
                var token = AuthHelper.GenerateJwtToken(employer.Id, "Employer");
                return Ok(new { Token = token });
            }
            return Unauthorized("Invalid password.");
        }

        [HttpGet("GetAllEmployers")]
        [Authorize(Roles = "Employer")]
        public IActionResult GetAllEmployers()
        {
            var employers = _context.Employers
                .ToList();

            if (employers == null)
            {
                return NotFound();
            }
            
            return Ok(employers);
        }

        [HttpGet("GetEmployerById")]
        public IActionResult GetEmployerById(int id)
        {
            var employer = _context.Employers
                .AsSplitQuery()
                .FirstOrDefault(c => c.Id == id);
            
            if (employer == null)
            {
                return NotFound();
            }
            
            return Ok(employer);
        }

        [HttpPost("GetEmployerAds")]
        public IActionResult GetEmployerAds(int id)
        {
            var employer = _context.Employers.FirstOrDefault(c => c.Id == id);

            if (employer == null)
            {
                return NotFound();
            }

            var ads = _context.Ads
                .Where(a => a.Employer.Id == id)
                .ToList();

            return Ok(ads);
        }

        [HttpPost("UpdateEmployer")]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> UpdateEmployer([FromBody] EmployerUpdateVM employerData)
        {
            int userId = AuthHelper.GetUserId(User);
            if (userId == 0)
            {
                return Unauthorized();
            }
            var employer = _context.Employers.FirstOrDefault(e => e.Id == userId);

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

            _context.Employers.Update(employer);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}