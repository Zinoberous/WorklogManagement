using System.Reflection;

namespace WorklogManagement.API.Chat;

public interface IModelDescriptionService
{
    /// <summary>
    /// Generiert die Beschreibung aller Modelle in einem bestimmten Namespace.
    /// </summary>
    /// <param name="assemblyName">Der Name der Assembly, die die Modelle enthält.</param>
    /// <param name="modelNamespace">Der Namespace, in dem sich die Modelle befinden.</param>
    /// <returns>Eine Liste von Modellbeschreibungen.</returns>
    IEnumerable<string> GenerateModelDescriptions(string assemblyName, string modelNamespace);

    /// <summary>
    /// Generiert die Beschreibung aller Enums in einer Assembly.
    /// </summary>
    /// <param name="assemblyName">Der Name der Assembly, die die Enums enthält.</param>
    /// <returns>Eine Liste von Enum-Beschreibungen.</returns>
    IEnumerable<string> GenerateEnumDescriptions(string assemblyName);
}

public class ModelDescriptionService : IModelDescriptionService
{
    /// <inheritdoc/>
    public IEnumerable<string> GenerateModelDescriptions(string assemblyName, string modelNamespace)
    {
        var assembly = Assembly.Load(assemblyName);
        var modelTypes = GetModelTypes(assembly, modelNamespace);

        var descriptions = new List<string>();
        foreach (var modelType in modelTypes)
        {
            descriptions.Add(GenerateModelDescription(modelType));
        }

        return descriptions;
    }

    /// <inheritdoc/>
    public IEnumerable<string> GenerateEnumDescriptions(string assemblyName)
    {
        var assembly = Assembly.Load(assemblyName);
        var enumTypes = GetEnumTypes(assembly);

        var descriptions = new List<string>();
        foreach (var enumType in enumTypes)
        {
            descriptions.Add(GenerateEnumDescription(enumType));
        }

        return descriptions;
    }

    private static IEnumerable<Type> GetModelTypes(Assembly assembly, string modelNamespace)
    {
        return assembly.GetTypes()
            .Where(t => t.Namespace == modelNamespace && t.IsClass);
    }

    private static IEnumerable<Type> GetEnumTypes(Assembly assembly)
    {
        return assembly.GetTypes()
            .Where(t => t.IsEnum);
    }

    private static string GenerateModelDescription(Type modelType)
    {
        var description = $"- {modelType.Name}:\n";

        // Nur Basistypen berücksichtigen
        foreach (var property in modelType.GetProperties())
        {
            if (IsPrimitive(property.PropertyType))
            {
                description += $"  - {property.Name} ({GetFriendlyTypeName(property.PropertyType)})\n";
            }
        }

        return description.Trim();
    }

    private static string GenerateEnumDescription(Type enumType)
    {
        var description = $"- {enumType.Name}:\n";
        foreach (var value in Enum.GetValues(enumType))
        {
            description += $"  - {value} ({(int)value})\n";
        }
        return description.Trim();
    }

    private static bool IsPrimitive(Type type)
    {
        var realType = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)
            ? Nullable.GetUnderlyingType(type)!
            : type;

        return
            realType.IsPrimitive
            || realType == typeof(string)
            || realType == typeof(DateTime)
            || realType == typeof(decimal);
    }

    private static string GetFriendlyTypeName(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return $"{Nullable.GetUnderlyingType(type)?.Name}?";
        }
        return type.Name;
    }
}
