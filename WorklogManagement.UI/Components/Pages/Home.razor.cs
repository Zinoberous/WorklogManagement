namespace WorklogManagement.UI.Components.Pages;

public partial class Home
{
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ViewModel.LoadOvertimeAsync();
        }
    }
}
