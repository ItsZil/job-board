using job_board.Models;
using job_board.Utilities;
using job_board.ViewModels;
using job_board.ViewModels.Company;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace job_board.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly AppDbContext _context;

        public AuthController(ILogger<AuthController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterCandidate([FromBody] CandidateRegistrationVM registrationData)
        {
            var hashedPasswordAndSalt = AuthHelper.HashPassword(registrationData.Password);

            var candidate = new Candidate
            {
                Email = registrationData.Email,
                FirstName = registrationData.FirstName,
                LastName = registrationData.LastName,
                Salt = hashedPasswordAndSalt.salt,
                Password = hashedPasswordAndSalt.hashedPassword
            };

            try
            {
                _context.Candidates.Add(candidate);
                await _context.SaveChangesAsync();

                var token = AuthHelper.GenerateJwtToken(candidate.Id, "Candidate");
                return Created(string.Empty, new { Token = token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginVM loginData)
        {
            string role = loginData.Role.ToLower();
            string salt = string.Empty;
            string hashedPassword = string.Empty;
            int userId = 0;
            
            if (role == "candidate")
            {
                var candidate = await _context.Candidates
                    .Where(c => c.Email == loginData.Email)
                    .FirstOrDefaultAsync();

                if (candidate == null)
                {
                    return NotFound("Account not found.");
                }

                userId = candidate.Id;
                salt = candidate.Salt;
                hashedPassword = candidate.Password;
            }
            else if (role == "company")
            {
                var company = await _context.Companies
                    .Where(c => c.Email == loginData.Email)
                    .FirstOrDefaultAsync();

                if (company == null)
                {
                    return NotFound("Account not found.");
                }

                userId = company.Id;
                salt = company.Salt;
                hashedPassword = company.Password;
            }
            else if (role == "admin")
            {
                var token = AuthHelper.GenerateJwtToken(-1, "admin");
                return Created(string.Empty, new { Token = token });
            }

            if (AuthHelper.DoesPasswordMatch(loginData.Password, salt, hashedPassword))
            {
                var token = AuthHelper.GenerateJwtToken(userId, role);
                return Created(string.Empty, new { Token = token });
            }
            return Unauthorized("Invalid password.");
        }
    }
}
