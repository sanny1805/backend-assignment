using Microsoft.EntityFrameworkCore;
using OT.Assessment.Domain.Models;
using OT.Assessment.Infrastructure.Database;
using OT.Assessment.Infrastructure.Interfaces;

namespace OT.Assessment.Infrastructure.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly DataContext _context;

        public PlayerRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Player> GetPlayerAsync(Guid accountId)
        {
            return await _context.Players
                .Include(x => x.CasinoWagers)
                .FirstOrDefaultAsync(p => p.AccountId == accountId);
        }

        public async Task<Player> CreatePlayerAsync(Player player)
        {
            await _context.Players.AddAsync(player);

            await _context.SaveChangesAsync();

            return player;
        }

        public async Task<IEnumerable<Player>> GetPlayersAsync()
        {
            return await _context.Players
                .Include(x => x.CasinoWagers)
                .ToListAsync();
        }

    }
}
