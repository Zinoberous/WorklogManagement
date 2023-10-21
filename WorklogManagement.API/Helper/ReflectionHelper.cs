namespace WorklogManagement.API.Helper
{
    internal static class ReflectionHelper
    {
        internal static object? GetPropertyValueByName(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName)?.GetValue(obj);
        }
    }
}
