using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OT.Assessment.App.Services;
using OT.Assessment.App.Services.Interfaces;
using OT.Assessment.App.ViewModels;
using OT.Assessment.Domain.Models;
using Serilog;
namespace OT.Assessment.App.Controllers
{
  
    [ApiController]
    [Route("api/player")]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        private readonly Serilog.ILogger _logger;
        private readonly IMapper _mapper;

        public PlayerController(IPlayerService playerService, IMapper mapper)
        {
           _playerService = playerService;
            _logger = Log.ForContext<PlayerController>();
            _mapper = mapper;
        }

        [HttpPost("casinowager")]
        public async Task<IActionResult> SubmitCasinoWager([FromBody] CasinoWagerViewModel wager)
        {
            _logger.Information("Received wager submission for AccountId: {AccountId}", wager.AccountId);
            try
            {
                var wagerCasino = _mapper.Map<CasinoWager>(wager);
                var result = await _playerService.AddCasinoWagerAsync(wagerCasino);

                if (string.IsNullOrEmpty(result))
                {
                    return BadRequest($"Somthing went wrong while saving the wager for AccountId: {wager.AccountId}");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error saving wager for AccountId: {AccountId}", wager.AccountId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{playerId}/wagers")]
        public async Task<IActionResult> GetPlayerCasinoWagers(Guid playerId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var wager = await _playerService.GetPlayerCasinoWagersAsync(playerId, page, pageSize);

                return Ok(wager);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error retrieving wager for AccountId: {playerId}", playerId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("topSpenders")]
        public async Task<IActionResult> GetTopSpenders([FromQuery] int count = 10)
        {
            try
            {
                var topSpenders = await _playerService.GetTopSpendersAsync(count);

                return Ok(topSpenders);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error retrieving top senders");
                return StatusCode(500, "Internal server error");
            };
        }
    }
}
