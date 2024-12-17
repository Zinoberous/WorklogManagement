using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Pages.Worklog;

public partial class Worklog
{
    [Parameter]
    public required int Id { get; set; }
}