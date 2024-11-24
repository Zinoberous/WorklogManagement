using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace WorklogManagement.UI.Components.Shared;

public partial class Editor
{
    private RadzenHtmlEditor _editor = null!;

    [Parameter]
    public string Value { get; set; } = string.Empty;

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("initializeEditorTabKeyHandling", _editor.Element);
        }
    }

    private async Task OnExecute(HtmlEditorExecuteEventArgs args)
    {
        switch (args.CommandName)
        {
            case "InsertNewLine":
                await InsertNewLine(args.Editor);
                await SynchronizeEditorValue();
                break;
            case "InsertCode":
                await InsertCode(args.Editor);
                await SynchronizeEditorValue();
                break;
        }
    }

    private async Task InsertNewLine(RadzenHtmlEditor editor)
    {
        const string newline = "<div><br></div>";

        await JSRuntime.InvokeVoidAsync("insertEditorHtml", editor.Element, newline);
    }

    private async Task InsertCode(RadzenHtmlEditor editor)
    {
        const string codeblock = "<pre><code><br/></code></pre>";

        await JSRuntime.InvokeVoidAsync("insertEditorHtml", editor.Element, codeblock);
    }

    private async Task SynchronizeEditorValue()
    {
        // Zero-Width Space hinzufügen, um durch editor.ExecuteCommandAsync im Hintergrund benötigte Update-Logik auszuführen
        await _editor.ExecuteCommandAsync(HtmlEditorCommands.InsertHtml, "&#8203;");
    }
}
