using Microsoft.EntityFrameworkCore;
using WorklogManagement.Data.Context;
using DB = WorklogManagement.Data.Models;

namespace WorklogManagement.Service.Models;

public class Absence : IDataModel
{
    private int? _id;
    public int? Id { get => _id; init => _id = value; }

    public required Enums.AbsenceType Type { get; init; }

    public required DateOnly Date { get; init; }

    public required int DurationMinutes { get; init; }

    public string? Note { get; init; }

    internal static Absence Map(DB.Absence absence)
    {
        return new()
        {
            Id = absence.Id,
            Type = (Enums.AbsenceType)absence.AbsenceTypeId,
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
