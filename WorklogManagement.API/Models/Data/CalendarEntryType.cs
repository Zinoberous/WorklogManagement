using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DB = WorklogManagement.DataAccess.Models;

namespace WorklogManagement.API.Models.Data
{
    public class CalendarEntryType(DB.CalendarEntryType calendarEntryType) : IData
    {
        [JsonPropertyName("id")]
        public int Id { get; } = calendarEntryType.Id;

        [JsonPropertyName("name")]
        [MaxLength(255)]
        public string Name { get; } = calendarEntryType.Name;
    }
}
