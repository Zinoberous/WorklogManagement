using System.ComponentModel.DataAnnotations;

namespace WorklogManagement.API.Models.Filter
{
    public class TicketFilter : IFilter
    {
        public int? RefId { get; set; }

        [StringLength(255)]
        public string? Title { get; set; } = null!;

        public int? StatusId { get; set; }
    }
}
