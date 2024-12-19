namespace WorklogManagement.API.Worklogs;

internal static class WorklogEndpoints
{
    internal static IEndpointRouteBuilder RegisterWorklogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/worklogs").WithTags("Worklogs");



        return app;
    }
}
