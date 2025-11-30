using Microsoft.EntityFrameworkCore;
using WorklogManagement.API.Models;
using WorklogManagement.Data.Context;
using DB = WorklogManagement.Data.Models;

namespace WorklogManagement.API.Worklogs;

internal static class WorklogEndpoints
{
    private const string IdEndpointPattern = "/{id}";

    internal static IEndpointRouteBuilder RegisterWorklogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/worklogs").WithTags("Worklogs");

        group.MapGet("", GetWorklogsAsync);
        group.MapGet(IdEndpointPattern, GetWorklogByIdAsync);
        group.MapPost("", SaveWorklogAsync);
        group.MapDelete(IdEndpointPattern, DeleteWorklogAsync);

        var attachmentGroup = group.MapGroup("/ticketAttachments").WithTags("TicketAttachments");

        attachmentGroup.MapGet("", GetAttachmentsAsync);
        attachmentGroup.MapGet(IdEndpointPattern, GetAttachmentByIdAsync);
        attachmentGroup.MapPost("", SaveAttachmentAsync);
        attachmentGroup.MapDelete(IdEndpointPattern, DeleteAttachmentAsync);

        return app;
    }

    #region worklog

    private static IQueryable<DB.Worklog> GetWorklogsQuery(WorklogManagementContext context)
    {
        return context.Worklogs
            .Include(x => x.Ticket)
            .Include(x => x.WorklogAttachments);
    }

    private static async Task<Page<Worklog>> GetWorklogsAsync(WorklogManagementContext context, string sortBy = "Id", int pageSize = 0, int pageIndex = 0, string? filter = null)
    {
        var items = GetWorklogsQuery(context);

        var page = Page.GetQuery(items, out var totalItems, out var totalPages, ref pageIndex, pageSize, sortBy, filter, Worklog.PropertyMappings);

        return new()
        {
            SortBy = sortBy,
            PageSize = pageSize,
            PageIndex = pageIndex,
            TotalPages = totalPages,
            TotalItems = totalItems,
            Items = await page
                .Select(x => Worklog.Map(x))
                .ToListAsync(),
        };
    }

    private static async Task<Worklog> GetWorklogByIdAsync(WorklogManagementContext context, int id)
    {
        var item = await GetWorklogsQuery(context)
            .SingleAsync(x => x.Id == id);

        return Worklog.Map(item);
    }

    private static async Task<Worklog> SaveWorklogAsync(WorklogManagementContext context, Worklog item)
    {
        await item.SaveAsync(context);

        return item;
    }

    private static async Task DeleteWorklogAsync(WorklogManagementContext context, int id)
    {
        await Worklog.DeleteAsync(context, id);
    }

    #endregion

    #region attachment

    private static async Task<Page<WorklogAttachment>> GetAttachmentsAsync(WorklogManagementContext context, string sortBy = "Id", int pageSize = 0, int pageIndex = 0, string? filter = null)
    {
        var items = context.WorklogAttachments;

        var page = Page.GetQuery(items, out var totalItems, out var totalPages, ref pageIndex, pageSize, sortBy, filter, WorklogAttachment.PropertyMappings);

        return new()
        {
            SortBy = sortBy,
            PageSize = pageSize,
            PageIndex = pageIndex,
            TotalPages = totalPages,
            TotalItems = totalItems,
            Items = await page
                .Select(x => WorklogAttachment.Map(x))
                .ToListAsync(),
        };
    }

    private static async Task<WorklogAttachment> GetAttachmentByIdAsync(WorklogManagementContext context, int id)
    {
        var item = await context.WorklogAttachments
            .SingleAsync(x => x.Id == id);

        return WorklogAttachment.Map(item);
    }

    private static async Task<WorklogAttachment> SaveAttachmentAsync(WorklogManagementContext context, WorklogAttachment item)
    {
        await item.SaveAsync(context);

        return item;
    }

    private static async Task DeleteAttachmentAsync(WorklogManagementContext context, int id)
    {
        await WorklogAttachment.DeleteAsync(context, id);
    }

    #endregion
}
