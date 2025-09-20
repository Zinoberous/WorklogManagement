using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using System.Text.RegularExpressions;

namespace WorklogManagement.UI.Components.Shared;

public partial class Editor
{
    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public string? Placeholder { get; set; }

    [Parameter]
    public string Value { get; set; } = string.Empty;

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = null!;

    private RadzenHtmlEditor _editor = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("initializeEditorTabKeyHandling", _editor.Element);
        }
    }

    private void OnValueChanged(string value)
    {
        ValueChanged.InvokeAsync(IsHtmlEmpty(value) ? null : value);
    }

    private static bool IsHtmlEmpty(string html)
    {
        // Entferne alle HTML-Tags
        var textContent = Regex.Replace(html, "<[^>]*>", "", RegexOptions.None, TimeSpan.FromSeconds(1));

        // Falls nach dem Entfernen der Tags kein Text übrig bleibt, ist der Editor "leer"
        return string.IsNullOrEmpty(textContent);
    }

    private static async Task OnExecute(HtmlEditorExecuteEventArgs args)
    {
        switch (args.CommandName)
        {
            case "InsertNewLine":
                await args.Editor.ExecuteCommandAsync(HtmlEditorCommands.InsertHtml, "<div><br></div>");
                break;
            case "InsertCode":
                await args.Editor.ExecuteCommandAsync(HtmlEditorCommands.InsertHtml, "<pre><code><br/></code></pre>");
                break;
        }
    }
}
