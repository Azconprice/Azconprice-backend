namespace Application.Models.DTOs
{
    public class DashboardStatistics
    {
        public int UserCount { get; set; }
        public int UploadedFilesCount { get; set; }
        public int RequestsCount { get; set; }
        public double TotalMoneyAmount { get; set; }
    }
}
