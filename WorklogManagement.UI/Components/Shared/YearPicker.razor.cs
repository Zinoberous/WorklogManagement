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
    public int Year { get; set; }

    public bool IsOpen { get; set; }
    public bool IsMouseInside { get; set; }

    public int PageIndex { get; set; }
    public int StartYear => PageIndex * ElementsPerPage + FirstYear;
    public int EndYear => StartYear + ElementsPerPage - FirstYear;

    public void Open()
    {
        PageIndex = (Year - FirstYear) / ElementsPerPage;

        IsOpen = true;
    }

    public void MouseIn() => IsMouseInside = true;

    public void MouseOut() => IsMouseInside = false;

    public void PrevPage() => PageIndex--;

    public void NextPage() => PageIndex++;

    public void Select(int year) => Year = year;

    public void Close() => IsOpen = false;

    public async Task TryCloseAsync()
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