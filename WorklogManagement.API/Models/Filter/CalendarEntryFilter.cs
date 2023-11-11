using System.Text.Json.Serialization;
using WorklogManagement.API.Enums;

namespace WorklogManagement.API.Models.Filter
{
    public class CalendarEntryFilter : IFilter
    {
        [JsonPropertyName("date")]
        public DateTime? Date { get; set; }

        [JsonPropertyName("from")]
        public DateTime? From { get; set; }

        [JsonPropertyName("to")]
        public DateTime? To { get; set; }

        [JsonPropertyName("calendarEntryType")]
        public string? CalendarEntryType { get; set; }

        public CalendarEntryType? CalendarEntryTypeEnum => string.IsNullOrWhiteSpace(CalendarEntryType) ? null : Enum.Parse<CalendarEntryType>(CalendarEntryType);
    }
}
