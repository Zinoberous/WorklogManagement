using Microsoft.AspNetCore.Components;
using System.Web;

namespace WorklogManagement.UI.Services;

public interface INavigator
{
    void UpdateQuery(string key, string? value);
}

public class Navigator(NavigationManager navigationManager) : INavigator
{
    private readonly NavigationManager _navigationManager = navigationManager;

    public void UpdateQuery(string key, string? value)
    {
        Uri uri = new(_navigationManager.Uri);

        var queryParams = HttpUtility.ParseQueryString(uri.Query);

        if (string.IsNullOrWhiteSpace(value))
        {
            queryParams.Remove(key);
        }
        else
        {
            queryParams[key] = value;
        }

        var newUri = $"{uri.GetLeftPart(UriPartial.Path)}?{queryParams}";

        _navigationManager.NavigateTo(newUri, forceLoad: false);
    }
}
