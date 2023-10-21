namespace WorklogManagement.API.Models.Filter
{
    public class DayFilter : IFilter
    {
        public DateTime? Date { get; set; }

        public bool? IsMobile { get; set; }

        public Enums.Workload? Workload { get; set; }
    }
}
