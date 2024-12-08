using Microsoft.AspNetCore.Components;
using Radzen;

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

    private void DateRender(DateRenderEventArgs args)
    {
        if (ViewModel.DatesWithEntries.Contains(DateOnly.FromDateTime(args.Date.Date)))
        {
            args.Attributes.Add("style", "background: var(--rz-primary);");
        }
    }
}
