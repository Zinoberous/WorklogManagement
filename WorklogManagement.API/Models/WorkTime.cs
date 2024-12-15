using Microsoft.EntityFrameworkCore;
using WorklogManagement.Data.Context;
using WorklogManagement.Shared.Enums;
using DB = WorklogManagement.Data.Models;
using Shd = WorklogManagement.Shared.Models;

namespace WorklogManagement.API.Models;

public record WorkTime : Shd.WorkTime
{
    private int _id;
    public new int Id { get => _id; init => _id = value; }

    internal static WorkTime Map(DB.WorkTime workTime)
    {
        return new()
        {
            Id = workTime.Id,
            Type = (WorkTimeType)workTime.WorkTimeTypeId,
            Date = workTime.Date,
            Expected = workTime.ExpectedSpan,
            Actual = workTime.ActualSpan,
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
                ExpectedSpan = Expected,
                ActualSpan = Actual,
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
            workTime.ExpectedSpan = Expected;
            workTime.ActualSpan = Actual;
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
