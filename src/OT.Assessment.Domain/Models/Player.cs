namespace OT.Assessment.Domain.Models
{
    public class Player
    {
        public Guid AccountId { get; set; }
        public string Username { get; set; }
        public ICollection<CasinoWager> CasinoWagers { get; set; } = new List<CasinoWager>();
    }
}
