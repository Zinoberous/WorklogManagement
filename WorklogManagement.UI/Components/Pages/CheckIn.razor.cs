using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Pages;
public partial class CheckIn
{
    [Parameter]
    [SupplyParameterFromQuery(Name = "date")]
    public DateOnly? InitialDate { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ViewModel.InitAsync(InitialDate);
        }
    }
}