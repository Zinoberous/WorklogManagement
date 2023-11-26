using System.Text.Json.Serialization;
using WorklogManagement.API.Enums;

namespace WorklogManagement.API.Models.Filter
{
    public class CalendarEntryFilter : IFilter
    {
        [JsonPropertyName("date")]
        public DateOnly? Date { get; set; }

        [JsonPropertyName("from")]
        public DateOnly? From { get; set; }

        [JsonPropertyName("to")]
        public DateOnly? To { get; set; }

        [JsonPropertyName("calendarEntryType")]
        public string? CalendarEntryType { get; set; }

        public CalendarEntryType? CalendarEntryTypeEnum => string.IsNullOrWhiteSpace(CalendarEntryType) ? null : Enum.Parse<CalendarEntryType>(CalendarEntryType);
    }
}
