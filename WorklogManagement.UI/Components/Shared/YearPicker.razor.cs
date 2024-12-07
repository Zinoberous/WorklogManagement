using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace WorklogManagement.UI.Components.Shared;

public partial class YearPicker
{
    [Parameter]
    public int Value { get; set; }

    [Parameter]
    public EventCallback<int> ValueChanged { get; set; }

    [Parameter]
    public EventCallback<int> OnChanged { get; set; }

    private const int FirstYear = 1;
    private const int YearsPerRow = 4;
    private const int RowsPerPage = 3;
    private const int ElementsPerPage = YearsPerRow * RowsPerPage;

    private RadzenTextBox _input = null!;

    private bool _isOpen;
    private bool IsOpen
    {
        get => _isOpen;
        set
        {
            if (_isOpen != value)
            {
                _isOpen = value;

                if (_isOpen)
                {
                    PageIndex = (Value - FirstYear) / ElementsPerPage;
                }
            }
        }
    }

    private int _pageIndex;
    private int PageIndex
    {
        get => _pageIndex;
        set => _pageIndex = value;
    }

    private int FirstYearOfPage => PageIndex * ElementsPerPage + FirstYear;
    private int LastYearOfPage => FirstYearOfPage + ElementsPerPage - FirstYear;

    private bool IsMouseInside { get; set; }

    private void Open() => IsOpen = true;

    private void MouseIn() => IsMouseInside = true;

    private void MouseOut() => IsMouseInside = false;

    private void PrevPage() => PageIndex--;

    private void NextPage() => PageIndex++;

    private async Task Select(int year)
    {
        Value = year;

        await ValueChanged.InvokeAsync(Value);
        await OnChanged.InvokeAsync(Value);
    }

    private void Close() => IsOpen = false;

    private async Task TryClose()
    {
        if (!IsMouseInside)
        {
            Close();
        }
        else
        {
            await _input.FocusAsync();
        }
    }
}