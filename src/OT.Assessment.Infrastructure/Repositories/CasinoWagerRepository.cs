using Microsoft.EntityFrameworkCore;
using OT.Assessment.Domain.Models;
using OT.Assessment.Infrastructure.Database;
using OT.Assessment.Infrastructure.Interfaces;

namespace OT.Assessment.Infrastructure.Repositories
{
    public class CasinoWagerRepository : ICasinoWagerRepository
    {
        private readonly DataContext _context;

        public CasinoWagerRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<CasinoWager> CreateCasinoWagerAsync(CasinoWager casinoWager)
        {
            casinoWager.WagerId = Guid.NewGuid();
            casinoWager.CreatedDateTime = DateTime.UtcNow;

            await _context.CasinoWagers.AddAsync(casinoWager);
            await _context.SaveChangesAsync();

            return casinoWager;
        }

        public async Task<IEnumerable<CasinoWager>> GetCasinoWayersAsync(Guid playerId)
        {
            return await _context.CasinoWagers.Include(x => x.Player)
                .Where(x => x.AccountId == playerId)
                .ToListAsync();
        }
    }
}
