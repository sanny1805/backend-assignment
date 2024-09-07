using OT.Assessment.App.Services.Interfaces;
using OT.Assessment.App.ViewModels;
using OT.Assessment.Domain.Models;
using OT.Assessment.Infrastructure.Interfaces;
using Serilog;
using System.Linq;
using System.Numerics;


namespace OT.Assessment.App.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Serilog.ILogger _logger;

        public PlayerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = Log.ForContext<PlayerService>();
        }

        public async Task<string> AddCasinoWagerAsync(CasinoWager casinoWager)
        {
            try
            {
                if (casinoWager == null)
                {
                    _logger.Warning("SubmitCasinoWager called with null wager");
                    return string.Empty;
                }

                var player = await _unitOfWork.Players.GetPlayerAsync(casinoWager.AccountId);

                if (player == null)
                {
                    player = new Player 
                    {
                        AccountId = casinoWager.AccountId, 
                        Username = casinoWager.Username 
                    };

                   var newPlayer = await _unitOfWork.Players.CreatePlayerAsync(player);
                    _logger.Information("Created new player with AccountId: {AccountId}", newPlayer.AccountId);
                }

               var newCasinoWager = await _unitOfWork.CasinoWagers.CreateCasinoWagerAsync(casinoWager);

                string message = $"Successfully added new wager with WagerId: {newCasinoWager.WagerId} for AccountId: {newCasinoWager.AccountId}";
                _logger.Information(message);

                return message;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in SubmitCasinoWager");
                return string.Empty;
            }
        }

        public async Task<CasinoViewModel> GetPlayerCasinoWagersAsync(Guid playerId,int page = 1, int pageSize = 10)
        {
            try
            {
                if (page <= 0 || pageSize <= 0)
                {
                    _logger.Warning("Invalid pagination parameters: page={Page}, pageSize={PageSize}", page, pageSize);
                    return new CasinoViewModel();
                }

                var casinoWagers = await _unitOfWork.CasinoWagers.GetCasinoWayersAsync(playerId);

                if (casinoWagers.Count() == 0)
                {
                    _logger.Warning($"No casino wagers were found for AccountId: {playerId}");
                    return new CasinoViewModel();
                }

                var total = casinoWagers.Count();
                var totalPages = (int)Math.Ceiling((double)total / pageSize);

                var paginatedWagers = casinoWagers.OrderByDescending(w => w.CreatedDateTime)
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();



                var wagersViewModel = new List<WagerViewModel>();

                foreach (var casinoWager in paginatedWagers)
                {
                    var wager = new WagerViewModel()
                    {
                        WagerId = casinoWager.WagerId,
                        Game = casinoWager.GameName,
                        Provider = casinoWager.Provider,
                        Amount = casinoWager.Amount,
                        CreatedDate = casinoWager.CreatedDateTime,
                    };

                    wagersViewModel.Add(wager);
                }

                var result = new CasinoViewModel()
                {
                    Data = wagersViewModel,
                    Page = page,
                    PageSize = pageSize,
                    Total = total,
                    TotalPages = totalPages
                };

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in GetPlayerCasinoWagers");
                return null;
            }
        }

        public async Task<IEnumerable<TopSpenderViewModel>> GetTopSpendersAsync(int count)
        {
            try
            {
                if (count <= 0)
                {
                    _logger.Error("Page number and page size must be greater than zero.");
                    return Enumerable.Empty<TopSpenderViewModel>();
                }

                var players = await _unitOfWork.Players.GetPlayersAsync();

                if (players == null || !players.Any())
                {
                    _logger.Error("No players were found.");
                    return Enumerable.Empty<TopSpenderViewModel>();
                }

                var totalPlayers = players.Count();
                var topSenders = players.Select(p => new
                                       {
                                           p.AccountId,
                                           p.Username,
                                           TotalSpent = p.CasinoWagers.Sum(w => w.Amount)
                                       }).OrderByDescending(p => p.TotalSpent)
                                         .Take(count)
                                         .ToList();

                var topSendersViewModel = new List<TopSpenderViewModel>();

                foreach (var topSender in topSenders)
                {
                    var topSenderViewModel = new TopSpenderViewModel()
                    {
                        AccountId = topSender.AccountId,
                        Username = topSender.Username,
                        totalAmountSpend = topSender.TotalSpent
                    };

                    topSendersViewModel.Add(topSenderViewModel);
                }

                return topSendersViewModel;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error retrieving top spenders");
                return null;
            }
        }
    }
}
