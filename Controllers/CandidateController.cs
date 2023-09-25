using job_board.Models;
using job_board.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace job_board.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CandidateController : ControllerBase
    {
        private readonly ILogger<CandidateController> _logger;
        private readonly AppDbContext _context;

        public CandidateController(ILogger<CandidateController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Candidate> Get()
        {
            var candidates = _context.Candidates.ToList();

            return candidates;
        }
    }
}