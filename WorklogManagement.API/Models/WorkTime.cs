using Microsoft.EntityFrameworkCore;
using WorklogManagement.Data.Context;
using WorklogManagement.Shared.Enums;
using DB = WorklogManagement.Data.Models;
using Shd = WorklogManagement.Shared.Models;

namespace WorklogManagement.API.Models;

public class WorkTime : Shd.WorkTime
{
    private int? _id;
    public new int? Id { get => _id; init => _id = value; }

    internal static WorkTime Map(DB.WorkTime workTime)
    {
        return new()
        {
            Id = workTime.Id,
            Type = (WorkTimeType)workTime.WorkTimeTypeId,
            Date = workTime.Date,
            ExpectedMinutes = workTime.ExpectedMinutes,
            ActualMinutes = workTime.ActualMinutes,
            Note = workTime.Note,
        };
    }

    internal async Task SaveAsync(WorklogManagementContext context)
    {
        DB.WorkTime workTime;

        if (_id is null)
        {
            workTime = new()
            {
                WorkTimeTypeId = (int)Type,
                Date = Date,
                ExpectedMinutes = ExpectedMinutes,
                ActualMinutes = ActualMinutes,
                Note = Note,
            };

            await context.WorkTimes.AddAsync(workTime);

            await context.SaveChangesAsync();

            _id = workTime.Id;
        }
        else
        {
            workTime = await context.WorkTimes.SingleAsync(x => x.Id == _id);

            workTime.WorkTimeTypeId = (int)Type;
            workTime.Date = Date;
            workTime.ExpectedMinutes = ExpectedMinutes;
            workTime.ActualMinutes = ActualMinutes;
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
