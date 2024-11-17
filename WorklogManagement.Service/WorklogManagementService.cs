using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WorklogManagement.Data.Context;
using WorklogManagement.Service.Enums;
using WorklogManagement.Service.Models;
using WorklogManagement.Service.Models.Queries;

namespace WorklogManagement.Service;

public class WorklogManagementService(WorklogManagementContext context) : IWorklogManagementService
{
    private readonly WorklogManagementContext _context = context;

    public async Task<TDataModel> SaveAsync<TDataModel>(TDataModel item)
        where TDataModel : IDataModel
    {
        await (Task)typeof(TDataModel)
            .GetMethod("SaveAsync", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(item, [_context])!;

        return item;
    }

    public async Task DeleteAsync<TDataModel>(int id)
        where TDataModel : IDataModel
    {
        await (Task)typeof(TDataModel)
            .GetMethod("DeleteAsync", BindingFlags.Static | BindingFlags.NonPublic)!
            .Invoke(null, [_context, id])!;
    }

    public async Task<Ticket> GetTicketByIdAsync(int id)
    {
        var ticket = await _context.Tickets
            .Include(x => x.TicketStatusLogs)
            .Include(x => x.TicketAttachments)
            .Include(x => x.Worklogs)
            .SingleAsync(x => x.Id == id);

        return Ticket.Map(ticket);
    }

    public async Task<Page<Ticket>> GetTicketsAsync(TicketQuery query)
    {
        var items = _context.Tickets
            .Include(x => x.TicketStatusLogs)
            .Include(x => x.TicketAttachments)
            .Where(x =>
                (query.RefId == null || (query.RefId == -1 && x.RefId == null) || (query.RefId == -2 && x.RefId != null) || x.RefId == query.RefId) &&
                (query.Title == null || x.Title.Contains(query.Title)) &&
                (query.Search == null || x.Title.Contains(query.Search) || (!string.IsNullOrWhiteSpace(x.Description) && x.Description.Contains(query.Search))) &&
                (query.Status == null || query.Status.Contains((TicketStatus)x.TicketStatusId))
            )
            .AsQueryable();

        var totalItems = await items.CountAsync();

        var totalPages = query.PageSize == 0 ? 1 : (int)Math.Ceiling((decimal)totalItems / query.PageSize);

        var page = query.PageSize == 0 ? items : items
            .Skip(query.PageSize * query.PageIndex)
            .Take(query.PageSize);

        var tickets = await page
            .Select(x => Ticket.Map(x))
            .ToListAsync();

        return new()
        {
            PageSize = query.PageSize,
            PageIndex = query.PageIndex,
            TotalPages = totalPages,
            TotalItems = totalItems,
            Items = tickets
        };
    }

    public async Task<Worklog> GetWorklogByIdAsync(int id)
    {
        var worklog = await _context.Worklogs
            .Include(x => x.Ticket)
            .Include(x => x.WorklogAttachments)
            .SingleAsync(x => x.Id == id);

        return Worklog.Map(worklog);
    }

    public async Task<Page<Worklog>> GetWorklogsAsync(WorklogQuery query)
    {
        var items = _context.Worklogs
            .Include(x => x.Ticket)
            .Include(x => x.WorklogAttachments)
            .Where(x =>
                (query.Date == null || x.Date == query.Date.Value) &&
                (query.TicketId == null || x.TicketId == query.TicketId) &&
                (query.Search == null || x.Ticket.Title.Contains(query.Search) || (!string.IsNullOrWhiteSpace(x.Description) && x.Description.Contains(query.Search)))
            )
            .AsQueryable();

        var totalItems = await items.CountAsync();

        var totalPages = query.PageSize == 0 ? 1 : (int)Math.Ceiling((decimal)totalItems / query.PageSize);

        var page = query.PageSize == 0 ? items : items
            .Skip(query.PageSize * query.PageIndex)
            .Take(query.PageSize);

        var worklogs = await page
            .Select(x => Worklog.Map(x))
            .ToListAsync();

        return new()
        {
            PageSize = query.PageSize,
            PageIndex = query.PageIndex,
            TotalPages = totalPages,
            TotalItems = totalItems,
            Items = worklogs
        };
    }

    public async Task<OvertimeInfo> GetOvertimeAsync()
    {
        var totalOvertimeMinutes = 0;
        var officeOvertimeMinutes = 0;
        var mobileOvertimeMinutes = 0;

        var workTimes = await _context.WorkTimes
            .Where(x => x.ActualMinutes != x.ExpectedMinutes)
            .ToListAsync();

        Parallel.ForEach(workTimes, entry =>
        {
            var expectedMinutes = entry.ExpectedMinutes;
            var actualMinutes = entry.ActualMinutes;

            var overtimeMinutes = (actualMinutes - expectedMinutes);

            Interlocked.Add(ref totalOvertimeMinutes, overtimeMinutes);

            if (entry.WorkTimeTypeId == (int)WorkTimeType.Office)
            {
                Interlocked.Add(ref officeOvertimeMinutes, overtimeMinutes);
            }
            else if (entry.WorkTimeTypeId == (int)WorkTimeType.Mobile)
            {
                Interlocked.Add(ref mobileOvertimeMinutes, overtimeMinutes);
            }
        });

        return new()
        {
            TotalMinutes = totalOvertimeMinutes,
            OfficeMinutes = officeOvertimeMinutes,
            MobileMinutes = mobileOvertimeMinutes,
        };
    }
}
