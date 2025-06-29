using Microsoft.AspNetCore.Components;
using System.Web;

namespace WorklogManagement.UI.Services;

public interface INavigationService
{
    void NavigateToPage(string uri);

    void UpdateQuery(string key, string? value);
}

public class NavigationService(NavigationManager navigationManager) : INavigationService
{
    private readonly NavigationManager _navigationManager = navigationManager;

    public void NavigateToPage(string uri)
    {
        _navigationManager.NavigateTo(uri, true);
    }

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
