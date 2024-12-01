using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace WorklogManagement.UI.Components.Shared;

public partial class YearPicker
{
    private RadzenTextBox _input = null!;

    private const int FirstYear = 1;
    private const int YearsPerRow = 4;
    private const int RowsPerPage = 3;
    private const int ElementsPerPage = YearsPerRow * RowsPerPage;

    [Parameter]
    public int Value { get; set; }

    [Parameter]
    public EventCallback<int> ValueChanged { get; set; }

    private bool _isOpen;
    private bool _isMouseInside;

    private int _pageIndex;
    private int _firstYearOfPage;
    private int _lastYearOfPage;

    private void Open()
    {
        SetPageIndex((Value - FirstYear) / ElementsPerPage);

        _isOpen = true;
    }

    private void MouseIn() => _isMouseInside = true;

    private void MouseOut() => _isMouseInside = false;

    private void SetPageIndex(int pageIndex)
    {
        _pageIndex = pageIndex;
        _firstYearOfPage = _pageIndex * ElementsPerPage + FirstYear;
        _lastYearOfPage = _firstYearOfPage + ElementsPerPage - FirstYear;
    }

    private void PrevPage() => SetPageIndex(_pageIndex - 1);

    private void NextPage() => SetPageIndex(_pageIndex + 1);

    private async Task Select(int year)
    {
        Value = year;

        await ValueChanged.InvokeAsync(Value);
    }

    private void Close() => _isOpen = false;

    private async Task TryClose()
    {
        if (!_isMouseInside)
        {
            Close();
        }
        else
        {
            await _input.FocusAsync();
        }
    }
}