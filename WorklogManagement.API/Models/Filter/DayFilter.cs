namespace WorklogManagement.API.Models.Filter
{
    public class DayFilter : IFilter
    {
        public DateTime? Date { get; set; }

        public bool? IsMobile { get; set; }

        public int? WorkloadId { get; set; }
    }
}
