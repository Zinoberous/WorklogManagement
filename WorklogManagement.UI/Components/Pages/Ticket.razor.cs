using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Pages
{
    public partial class Ticket
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
