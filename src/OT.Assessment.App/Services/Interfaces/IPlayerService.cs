using OT.Assessment.App.ViewModels;
using OT.Assessment.Domain.Models;

namespace OT.Assessment.App.Services.Interfaces
{
    public interface IPlayerService
    {
        Task<string> AddCasinoWagerAsync(CasinoWager casinoWager);
        Task<CasinoViewModel> GetPlayerCasinoWagersAsync(Guid playerId, int page = 1, int pageSize = 10);
        Task<IEnumerable<TopSpenderViewModel>> GetTopSpendersAsync(int count);
    }
}
