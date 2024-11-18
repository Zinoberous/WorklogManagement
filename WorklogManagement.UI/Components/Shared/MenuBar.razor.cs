using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Shared
{
    public partial class MenuBar
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
