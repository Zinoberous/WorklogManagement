using Microsoft.EntityFrameworkCore;
using WorklogManagement.Data.Context;
using WorklogManagement.Shared.Enums;
using DB = WorklogManagement.Data.Models;
using DTO = WorklogManagement.Shared.Models;

namespace WorklogManagement.API.WorkTimes;

public record WorkTime : DTO.WorkTime
{
    private int _id;
    public new int Id { get => _id; init => _id = value; }

    // DTO > DB
    internal static Dictionary<string, string> PropertyMappings { get; } = new()
    {
        { "Id", "Id" },
        { "Type", "WorkTimeTypeId" },
        { "Date", "Date" },
        { "Expected", "ExpectedSeconds" },
        { "Actual", "ActualSeconds" },
        { "Note", "Note" },
    };

    internal static WorkTime Map(DB.WorkTime workTime)
    {
        return new()
        {
            Id = workTime.Id,
            Type = (WorkTimeType)workTime.WorkTimeTypeId,
            Date = workTime.Date,
            Expected = workTime.Expected,
            Actual = workTime.Actual,
            Note = workTime.Note,
        };
    }

    internal async Task SaveAsync(WorklogManagementContext context)
    {
        var workTime = await context.WorkTimes.SingleOrDefaultAsync(x => x.Id == _id);

        if (workTime is null)
        {
            workTime = new()
            {
                WorkTimeTypeId = (int)Type,
                Date = Date,
                Expected = Expected,
                Actual = Actual,
                Note = Note,
            };

            await context.WorkTimes.AddAsync(workTime);

            await context.SaveChangesAsync();

            _id = workTime.Id;
        }
        else
        {
            workTime.WorkTimeTypeId = (int)Type;
            workTime.Date = Date;
            workTime.Expected = Expected;
            workTime.Actual = Actual;
            workTime.Note = Note;

            await context.SaveChangesAsync();
        }
    }

    internal static async Task DeleteAsync(WorklogManagementContext context, int id)
    {
        var workTime = await context.WorkTimes
            .SingleAsync(x => x.Id == id);

        context.WorkTimes.Remove(workTime);

        await context.SaveChangesAsync();
    }
}
