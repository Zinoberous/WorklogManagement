using Microsoft.EntityFrameworkCore;
using WorklogManagement.Data.Context;
using WorklogManagement.Shared.Enums;
using DB = WorklogManagement.Data.Models;
using DTO = WorklogManagement.Shared.Models;

namespace WorklogManagement.API.Absences;

public record Absence : DTO.Absence
{
    private int _id;
    public new int Id { get => _id; init => _id = value; }

    // DTO > DB
    internal static Dictionary<string, string> PropertyMappings { get; } = new()
    {
        { "Id", "Id" },
        { "Type", "AbsenceTypeId" },
        { "Title", "Title" },
        { "Date", "Date" },
        { "Duration", "DurationSeconds" },
        { "Note", "Note" },
    };

    internal static Absence Map(DB.Absence absence)
    {
        return new()
        {
            Id = absence.Id,
            Type = (AbsenceType)absence.AbsenceTypeId,
            Date = absence.Date,
            Duration = absence.Duration,
            Note = absence.Note
        };
    }

    internal async Task SaveAsync(WorklogManagementContext context)
    {
        var absence = await context.Absences.SingleOrDefaultAsync(x => x.Id == _id);

        if (absence is null)
        {
            absence = new()
            {
                AbsenceTypeId = (int)Type,
                Date = Date,
                Duration = Duration,
                Note = Note,
            };

            await context.Absences.AddAsync(absence);

            await context.SaveChangesAsync();

            _id = absence.Id;
        }
        else
        {
            absence.AbsenceTypeId = (int)Type;
            absence.Date = Date;
            absence.Duration = Duration;
            absence.Note = Note;

            await context.SaveChangesAsync();
        }
    }

    internal static async Task DeleteAsync(WorklogManagementContext context, int id)
    {
        var absence = await context.Absences
            .SingleAsync(x => x.Id == id);

        context.Absences.Remove(absence);

        await context.SaveChangesAsync();
    }
}
