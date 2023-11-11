using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DB = WorklogManagement.DataAccess.Models;

namespace WorklogManagement.API.Models.Data
{
    public class CalendarEntryType : IData
    {
        [JsonPropertyName("id")]
        public int Id { get; }

        [JsonPropertyName("name")]
        [MaxLength(255)]
        public string Name { get; }

        public CalendarEntryType(DB.CalendarEntryType calendarEntryType)
        {
            Id = calendarEntryType.Id;
            Name = calendarEntryType.Name;
        }
    }
}
