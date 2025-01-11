using Microsoft.AspNetCore.Components;
using Radzen;
using WorklogManagement.Shared.Enums;
using WorklogManagement.UI.Common;
using WorklogManagement.UI.Services;

namespace WorklogManagement.UI.Components.Shared;

public partial class TicketStatusDropDown
{
    [Parameter]
    public TicketStatus Value { get; set; } = TicketStatus.Todo;

    [Parameter]
    public EventCallback<TicketStatus> ValueChanged { get; set; }

    [Inject]
    private ITicketStatusService TicketStatusService { get; set; } = null!;

    private IReadOnlyDictionary<TicketStatus, string> TicketStatusChangeOptions
        => TicketStatusService.GetNextStatusOptions(Value).ToDictionary(x => x, x => x.ToString());

    private string StatusStyle => $"color: {Constant.TicketStatusColor[Value]}; background-color: {Constant.TicketStatusBgColor[Value]}";

    private void StatusRender(DropDownItemRenderEventArgs<TicketStatus> args)
    {
        if (args.Item is KeyValuePair<TicketStatus, string> item)
        {
            var status = item.Key;

            if (status == Value)
            {
                args.Attributes.Add("style", "display: none;");
            }
            else
            {
                args.Attributes.Add("style", $"color: {Constant.TicketStatusColor[status]}; background-color: {Constant.TicketStatusBgColor[status]};");
            }
        }
    }
}
