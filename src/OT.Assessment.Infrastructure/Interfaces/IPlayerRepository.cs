using OT.Assessment.Domain.Models;

namespace OT.Assessment.Infrastructure.Interfaces
{
    public interface IPlayerRepository
    {
        Task<Player> GetPlayerAsync(Guid accountId);
        Task<Player> CreatePlayerAsync(Player player);
        Task<IEnumerable<Player>> GetPlayersAsync();
    }
}
