using System.ComponentModel.DataAnnotations;

namespace WorklogManagement.API.Models.Filter
{
    public class WorklogAttachmentFilter : IFilter
    {
        public int? WorklogId { get; set; }

        [StringLength(255)]
        public string? Name { get; set; } = null!;
    }
}
