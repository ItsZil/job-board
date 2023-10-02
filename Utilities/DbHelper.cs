using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace job_board.Utilities
{
    public class DbHelper
    {
        private readonly AppDbContext _context;

        public DbHelper(AppDbContext context)
        {
            _context = context;
        }

        public bool DoesCompanyExist(int companyId)
        {
            var company = _context.Companies
            .AsSplitQuery()
            .FirstOrDefault(c => c.Id == companyId);

            return company == null;
        }

        public bool DoesAdExist(int adId)
        {
            var ad = _context.Ads
            .AsSplitQuery()
            .FirstOrDefault(a => a.Id == adId);

            return ad == null;
        }

        public bool DoesCompanyAdExist(int companyId, int adId)
        {
            var ad = _context.Ads
            .AsSplitQuery()
            .FirstOrDefault(a => a.Id == adId && a.Company.Id == companyId);

            return ad == null;
        }
    }
}
