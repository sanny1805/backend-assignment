namespace OT.Assessment.App.ViewModels
{
    public class CasinoViewModel
    {
        public List<WagerViewModel> Data { get; set; } = new List<WagerViewModel>();
        public int Page {  get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public int TotalPages { get; set; }
    }
}
