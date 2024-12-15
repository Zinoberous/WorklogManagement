using Microsoft.EntityFrameworkCore;
using WorklogManagement.Data.Context;
using WorklogManagement.Shared.Enums;
using DB = WorklogManagement.Data.Models;
using Shd = WorklogManagement.Shared.Models;

namespace WorklogManagement.API.Models;

public record Absence : Shd.Absence
{
    private int _id;
    public new int Id { get => _id; init => _id = value; }

    internal static Absence Map(DB.Absence absence)
    {
        return new()
        {
            Id = absence.Id,
            Type = (AbsenceType)absence.AbsenceTypeId,
            Date = absence.Date,
            Duration = absence.DurationSpan,
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
                DurationSpan = Duration,
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
            absence.DurationSpan = Duration;
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
