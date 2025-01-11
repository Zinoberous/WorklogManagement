using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Pages.Tracking;

public partial class Tracking
{
    [Parameter]
    [SupplyParameterFromQuery(Name = "date")]
    public DateOnly? InitialDate { get; set; }

    [Parameter]
    [SupplyParameterFromQuery(Name = "search")]
    public string? InitialSearch { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        //await ViewModel.InitAsync(InitialDate, InitialSearch);
    }
}
