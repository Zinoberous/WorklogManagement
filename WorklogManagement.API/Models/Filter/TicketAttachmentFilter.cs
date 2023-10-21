using System.ComponentModel.DataAnnotations;

namespace WorklogManagement.API.Models.Filter
{
    public class TicketAttachmentFilter : IFilter
    {
        public int TicketId { get; set; }

        [StringLength(255)]
        public string Name { get; set; } = null!;
    }
}
