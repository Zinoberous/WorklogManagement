using Microsoft.EntityFrameworkCore;
using WorklogManagement.Data.Context;
using WorklogManagement.Shared.Enums;
using DB = WorklogManagement.Data.Models;
using Shd = WorklogManagement.Shared.Models;

namespace WorklogManagement.API.Models;

public record Absence : Shd.Absence
{
    private int? _id;
    public new int? Id { get => _id; init => _id = value; }

    internal static Absence Map(DB.Absence absence)
    {
        return new()
        {
            Id = absence.Id,
            Type = (AbsenceType)absence.AbsenceTypeId,
            Date = absence.Date,
            DurationMinutes = absence.DurationMinutes,
            Note = absence.Note,
        };
    }

    internal async Task SaveAsync(WorklogManagementContext context)
    {
        DB.Absence absence;

        if (_id is null)
        {
            absence = new()
            {
                AbsenceTypeId = (int)Type,
                Date = Date,
                DurationMinutes = DurationMinutes,
                Note = Note,
            };

            await context.Absences.AddAsync(absence);

            await context.SaveChangesAsync();

            _id = absence.Id;
        }
        else
        {
            absence = await context.Absences.SingleAsync(x => x.Id == _id);

            absence.AbsenceTypeId = (int)Type;
            absence.Date = Date;
            absence.DurationMinutes = DurationMinutes;
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
