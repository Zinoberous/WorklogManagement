using Microsoft.AspNetCore.Components;
using System.Web;

namespace WorklogManagement.UI.Helper;

internal static class NavigationHelper
{
    internal static void UpdateQuery(NavigationManager navigationManager, string key, string? value)
    {
        Uri uri = new(navigationManager.Uri);

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

        navigationManager.NavigateTo(newUri, forceLoad: false);
    }
}
