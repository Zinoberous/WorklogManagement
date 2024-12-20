using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using WorklogManagement.API.Models;
using WorklogManagement.Data.Context;
using DB = WorklogManagement.Data.Models;

namespace WorklogManagement.API.Tickets;

internal static class TicketEndpoints
{
    internal static IEndpointRouteBuilder RegisterTicketEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/tickets").WithTags("Tickets");

        group.MapGet("", GetTicketsAsync);
        group.MapGet("/{id}", GetTicketByIdAsync);
        group.MapPost("", SaveTicketAsync);
        group.MapDelete("/{id}", DeleteTicketAsync);

        var attachmentGroup = group.MapGroup("/ticketAttachments").WithTags("TicketAttachments");

        attachmentGroup.MapGet("", GetAttachmentsAsync);
        attachmentGroup.MapGet("/{id}", GetAttachmentByIdAsync);
        attachmentGroup.MapPost("", SaveAttachmentAsync);
        attachmentGroup.MapDelete("/{id}", DeleteAttachmentAsync);

        var statusLogGroup = group.MapGroup("/ticketStatusLogs").WithTags("TicketStatusLogs");

        statusLogGroup.MapGet("", GetStatusLogsAsync);
        statusLogGroup.MapGet("/{id}", GetStatusLogByIdAsync);
        statusLogGroup.MapPost("", SaveStatusLogAsync);

        return app;
    }

    #region ticket

    private static IQueryable<DB.Ticket> GetTicketsQuery(WorklogManagementContext context)
    {
        return context.Tickets
            .Include(x => x.TicketStatusLogs)
            .Include(x => x.TicketAttachments)
            .Include(x => x.Worklogs);
    }

    private static async Task<Page<Ticket>> GetTicketsAsync(WorklogManagementContext context, string sortBy = "Id", uint pageSize = 0, uint pageIndex = 0, string? filter = null)
    {
        var items = GetTicketsQuery(context);

        var page = Page.GetQuery(items, out var totalItems, out var totalPages, ref pageIndex, pageSize, sortBy, filter, Ticket.PropertyMappings);

        return new()
        {
            SortBy = sortBy,
            PageSize = pageSize,
            PageIndex = pageIndex,
            TotalPages = totalPages,
            TotalItems = totalItems,
            Items = await page
                .Select(x => Ticket.Map(x))
                .ToListAsync(),
        };
    }

    private static async Task<Ticket> GetTicketByIdAsync(WorklogManagementContext context, int id)
    {
        var item = await GetTicketsQuery(context)
            .SingleAsync(x => x.Id == id);

        return Ticket.Map(item);
    }

    private static async Task<Ticket> SaveTicketAsync(WorklogManagementContext context, Ticket item)
    {
        await item.SaveAsync(context);

        return item;
    }

    private static async Task DeleteTicketAsync(WorklogManagementContext context, int id)
    {
        await Ticket.DeleteAsync(context, id);
    }

    #endregion

    #region attachment

    private static async Task<Page<TicketAttachment>> GetAttachmentsAsync(WorklogManagementContext context, string sortBy = "Id", uint pageSize = 0, uint pageIndex = 0, string? filter = null)
    {
        var items = context.TicketAttachments;

        var page = Page.GetQuery(items, out var totalItems, out var totalPages, ref pageIndex, pageSize, sortBy, filter, TicketAttachment.PropertyMappings);

        return new()
        {
            SortBy = sortBy,
            PageSize = pageSize,
            PageIndex = pageIndex,
            TotalPages = totalPages,
            TotalItems = totalItems,
            Items = await page
                .Select(x => TicketAttachment.Map(x))
                .ToListAsync(),
        };
    }

    private static async Task<TicketAttachment> GetAttachmentByIdAsync(WorklogManagementContext context, int id)
    {
        var item = await context.TicketAttachments
            .SingleAsync(x => x.Id == id);

        return TicketAttachment.Map(item);
    }

    private static async Task<TicketAttachment> SaveAttachmentAsync(WorklogManagementContext context, TicketAttachment item)
    {
        await item.SaveAsync(context);

        return item;
    }

    private static async Task DeleteAttachmentAsync(WorklogManagementContext context, int id)
    {
        await TicketAttachment.DeleteAsync(context, id);
    }

    #endregion

    #region statuslog

    private static IQueryable<DB.TicketStatusLog> GetStatusLogsQuery(WorklogManagementContext context)
    {
        return context.TicketStatusLogs
            .Include(x => x.Ticket);
    }

    private static async Task<Page<TicketStatusLog>> GetStatusLogsAsync(WorklogManagementContext context, string sortBy = "Id", uint pageSize = 0, uint pageIndex = 0, string? filter = null)
    {
        var items = GetStatusLogsQuery(context);

        var page = Page.GetQuery(items, out var totalItems, out var totalPages, ref pageIndex, pageSize, sortBy, filter, TicketStatusLog.PropertyMappings);

        return new()
        {
            SortBy = sortBy,
            PageSize = pageSize,
            PageIndex = pageIndex,
            TotalPages = totalPages,
            TotalItems = totalItems,
            Items = await page
                .Select(x => TicketStatusLog.Map(x))
                .ToListAsync(),
        };
    }

    private static async Task<TicketStatusLog> GetStatusLogByIdAsync(WorklogManagementContext context, int id)
    {
        var item = await context.TicketStatusLogs
            .SingleAsync(x => x.Id == id);

        return TicketStatusLog.Map(item);
    }

    private static async Task<TicketStatusLog> SaveStatusLogAsync(WorklogManagementContext context, TicketStatusLog item)
    {
        await item.SaveAsync(context);

        return item;
    }

    #endregion
}
