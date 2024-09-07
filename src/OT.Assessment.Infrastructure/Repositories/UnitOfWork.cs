using OT.Assessment.Infrastructure.Interfaces;

namespace OT.Assessment.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public IPlayerRepository Players { get; }
        public ICasinoWagerRepository CasinoWagers { get; }

        public UnitOfWork(IPlayerRepository playerRepository, ICasinoWagerRepository casinoWagerRepository)
        {
            Players = playerRepository;
            CasinoWagers = casinoWagerRepository;
        }
    }
}
