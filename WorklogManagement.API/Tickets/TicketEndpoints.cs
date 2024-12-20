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

        var attachmentGroup = group.MapGroup("/{ticketId}/attachments");

        var statusGroup = group.MapGroup("/{ticketId}/status");

        return app;
    }

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
        var ticket = await GetTicketsQuery(context)
            .SingleAsync(x => x.Id == id);

        return Ticket.Map(ticket);
    }

    private static async Task<Ticket> SaveTicketAsync(WorklogManagementContext context, Ticket ticket)
    {
        await ticket.SaveAsync(context);

        return ticket;
    }

    private static async Task DeleteTicketAsync(WorklogManagementContext context, int id)
    {
        await Ticket.DeleteAsync(context, id);
    }
}
