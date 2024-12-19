namespace WorklogManagement.API.Tickets;

internal static class TicketEndpoints
{
    internal static IEndpointRouteBuilder RegisterTicketEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/tikets").WithTags("Tickets");

        

        return app;
    }
}
