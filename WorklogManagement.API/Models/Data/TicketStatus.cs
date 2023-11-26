using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DB = WorklogManagement.DataAccess.Models;

namespace WorklogManagement.API.Models.Data
{
    public class TicketStatus(DB.TicketStatus status) : IData
    {
        [JsonPropertyName("id")]
        public int Id { get; } = status.Id;

        [JsonPropertyName("name")]
        [MaxLength(255)]
        public string Name { get; } = status.Name;
    }
}
