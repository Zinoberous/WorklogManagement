using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace WorklogManagement.UI.Components.Shared
{
    public partial class Editor
    {
        private string _value = string.Empty;

        [Parameter]
        public string Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    ValueChanged.InvokeAsync(value);
                }
            }
        }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        private RadzenHtmlEditor editor;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("initializeEditorTabKeyHandling", editor.Element);
            }
        }

        async Task OnExecute(HtmlEditorExecuteEventArgs args)
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

        async Task InsertNewLine(RadzenHtmlEditor editor)
        {
            const string newline = "<div><br></div>";

            await JSRuntime.InvokeVoidAsync("insertEditorHtml", editor.Element, newline);
        }

        async Task InsertCode(RadzenHtmlEditor editor)
        {
            const string codeblock = "<pre><code><br/></code></pre>";

            await JSRuntime.InvokeVoidAsync("insertEditorHtml", editor.Element, codeblock);
        }

        private async Task SynchronizeEditorValue()
        {
            // Zero-Width Space hinzufügen, um durch editor.ExecuteCommandAsync im Hintergrund benötigte Update-Logik auszuführen
            await editor.ExecuteCommandAsync(HtmlEditorCommands.InsertHtml, "&#8203;");
        }
    }
}
