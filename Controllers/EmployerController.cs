using job_board.Models;
using job_board.Utilities;
using job_board.ViewModels;
using job_board.ViewModels.Employer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

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
            byte[] passwordPlain = Encoding.UTF8.GetBytes(registrationData.Password);
            byte[] passwordSalt = RandomNumberGenerator.GetBytes(32);

            string saltString, passwordHashString;
            using (var pbkdf2 = new Rfc2898DeriveBytes(passwordPlain, passwordSalt, 10000, HashAlgorithmName.SHA512))
            {
                byte[] hash = pbkdf2.GetBytes(32);
                saltString = Convert.ToBase64String(passwordSalt);
                passwordHashString = Convert.ToBase64String(hash);
            }

            var employer = new Employer
            {
                Email = registrationData.Email,
                Company = registrationData.Company,
                CompanyDescription = registrationData.CompanyDescription,
                Industry = registrationData.Industry,
                Website = registrationData.Website,
                Salt = saltString,
                Password = passwordHashString
            };

            try
            {
                _context.Employers.Add(employer);
                await _context.SaveChangesAsync();
                return Ok();
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

            byte[] passwordPlain = Encoding.UTF8.GetBytes(loginData.Password);
            byte[] passwordSalt = Convert.FromBase64String(employer.Salt);

            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(passwordPlain, passwordSalt, 10000, HashAlgorithmName.SHA512))
            {
                byte[] computedHash = rfc2898DeriveBytes.GetBytes(32);
                if (computedHash.SequenceEqual(Convert.FromBase64String(employer.Password)))
                {
                    return Ok("Authentication successful.");
                }
            }
            return Unauthorized("Invalid password.");
        }

        [HttpGet("GetAllEmployers")]
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
    }
}