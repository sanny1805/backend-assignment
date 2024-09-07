using OT.Assessment.Domain.Models;

namespace OT.Assessment.Infrastructure.Interfaces
{
    public interface ICasinoWagerRepository
    {
        Task<CasinoWager> CreateCasinoWagerAsync(CasinoWager casinoWager);
        Task<IEnumerable<CasinoWager>> GetCasinoWayersAsync(Guid playerId);
    }
}
