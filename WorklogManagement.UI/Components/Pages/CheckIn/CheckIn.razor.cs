using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Pages.CheckIn;

public partial class CheckIn
{
    [Parameter]
    [SupplyParameterFromQuery(Name = "date")]
    public DateOnly? InitialDate { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await ViewModel.InitAsync(InitialDate);
    }
}
