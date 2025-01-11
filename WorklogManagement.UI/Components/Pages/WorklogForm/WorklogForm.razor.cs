using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Pages.WorklogForm;

public partial class WorklogForm
{
    [Parameter]
    public required int Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        await ViewModel.LoadWorklogAsync(Id);
    }
}
