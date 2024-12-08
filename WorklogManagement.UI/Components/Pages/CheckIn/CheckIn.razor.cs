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
        ICollection<string> background = [];

        if (ViewModel.DatesWithWorkTimes.Contains(DateOnly.FromDateTime(args.Date.Date)))
        {
            background.Add("steelblue");
        }
        if (ViewModel.DatesWithAbsences.Contains(DateOnly.FromDateTime(args.Date.Date)))
        {
            background.Add("lightcoral");
        }

        switch (background.Count)
        {
            case 0:
                break;
            case 1:
                args.Attributes.Add("style", $"background: {background.ElementAt(0)};");
                break;
            default:
                args.Attributes.Add("style", $"background: linear-gradient(to right, {string.Join(", ", background)});");
                break;
        }
    }
}
