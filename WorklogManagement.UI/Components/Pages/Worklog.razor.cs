using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Pages
{
    public partial class Worklog
    {
        [Parameter]
        public required int Id { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {

            }
        }
    }
}
