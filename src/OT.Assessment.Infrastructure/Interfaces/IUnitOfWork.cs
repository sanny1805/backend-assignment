namespace OT.Assessment.Infrastructure.Interfaces
{
    public interface IUnitOfWork
    {
        IPlayerRepository Players { get; }
        ICasinoWagerRepository CasinoWagers { get; }
    }
}
