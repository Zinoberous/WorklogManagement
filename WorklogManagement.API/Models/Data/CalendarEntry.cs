﻿using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WorklogManagement.DataAccess.Context;
using DB = WorklogManagement.DataAccess.Models;

namespace WorklogManagement.API.Models.Data
{
    public class CalendarEntry : IData
    {
        // TODO: uri self

        [JsonPropertyName("id")]
        public int? Id { get; private set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("duration")]
        public TimeSpan Duration { get; set; }

        [JsonPropertyName("type")]
        public Enums.CalendarEntryType Type { get; set; }

        [JsonPropertyName("workloadComment")]
        public string? Note { get; set; }

        [JsonConstructor]
        public CalendarEntry(int? id, DateTime date, TimeSpan duration, Enums.CalendarEntryType type, string? note)
        {
            Id = id;
            Date = date;
            Duration = duration;
            Type = type;
            Note = note;
        }

        public CalendarEntry(DB.CalendarEntry calendarEntry)
        {
            Id = calendarEntry.Id;
            Date = calendarEntry.Date;
            Duration = calendarEntry.Duration;
            Type = (Enums.CalendarEntryType)calendarEntry.CalendarEntryTypeId;
            Note = calendarEntry.Note;
        }

        public static async Task<CalendarEntry> GetAsync(int id, WorklogManagementContext context)
        {
            return new(await context.CalendarEntries.SingleAsync(x => x.Id == id));
        }

        public async Task SaveAsync(WorklogManagementContext context)
        {
            DB.CalendarEntry calendarEntry;

            if (Id == default)
            {
                calendarEntry = new()
                {
                    Date = Date,
                    Duration = Duration,
                    CalendarEntryTypeId = (int)Type,
                    Note = Note,
                };

                await context.CalendarEntries.AddAsync(calendarEntry);

                await context.SaveChangesAsync();

                Id = calendarEntry.Id;
            }
            else
            {
                calendarEntry = await context.CalendarEntries.SingleAsync(x => x.Id == Id);

                calendarEntry.Date = Date;
                calendarEntry.Duration = Duration;
                calendarEntry.CalendarEntryTypeId = (int)Type;
                calendarEntry.Note = Note;

                await context.SaveChangesAsync();
            }
        }
    }
}