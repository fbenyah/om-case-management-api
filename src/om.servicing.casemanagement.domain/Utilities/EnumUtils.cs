namespace om.servicing.casemanagement.domain.Utilities;

public static class EnumUtils
{
    public static void ValidateEnumType<T>() where T : struct, Enum
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException("T must be an enumerated type");
        }
    }

    public static bool IsValidEnumValue<T>(string value, bool ignoreCase = true) where T : struct, Enum
    {
        ValidateEnumType<T>();
        return Enum.TryParse<T>(value, ignoreCase, out _);
    }

    public static List<string> GetEnumNames<T>() where T : struct, Enum
    {
        ValidateEnumType<T>();
        return Enum.GetNames(typeof(T)).ToList();
    }

    public static List<T> GetEnumValues<T>() where T : struct, Enum
    {
        ValidateEnumType<T>();
        return Enum.GetValues(typeof(T)).Cast<T>().ToList();
    }

    public static string GetEnumName<T>(T value) where T : struct, Enum
    {
        ValidateEnumType<T>();
        return Enum.GetName(typeof(T), value) ?? string.Empty;
    }

    public static T? GetEnumValueFromName<T>(string name, bool ignoreCase = true) where T : struct, Enum
    {
        ValidateEnumType<T>();
        if (Enum.TryParse<T>(name, ignoreCase, out var result))
        {
            return result;
        }
        return null;
    }

    public static Dictionary<string, T> GetEnumNameValueDictionary<T>() where T : struct, Enum
    {
        ValidateEnumType<T>();
        return Enum.GetValues(typeof(T))
                   .Cast<T>()
                   .ToDictionary(e => Enum.GetName(typeof(T), e) ?? string.Empty, e => e);
    }

    public static Dictionary<T, string> GetEnumValueNameDictionary<T>() where T : struct, Enum
    {
        ValidateEnumType<T>();
        return Enum.GetValues(typeof(T))
                   .Cast<T>()
                   .ToDictionary(e => e, e => Enum.GetName(typeof(T), e) ?? string.Empty);
    }

    public static string GetEnumDescription<T>(T value) where T : struct, Enum
    {
        ValidateEnumType<T>();
        var fieldInfo = typeof(T).GetField(value.ToString());
        var descriptionAttribute = fieldInfo?.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false)
                                             .FirstOrDefault() as System.ComponentModel.DescriptionAttribute;
        return descriptionAttribute?.Description ?? value.ToString();
    }

    public static List<string> GetEnumDescriptions<T>() where T : struct, Enum
    {
        ValidateEnumType<T>();
        return Enum.GetValues(typeof(T))
                   .Cast<T>()
                   .Select(e => GetEnumDescription(e))
                   .ToList();
    }

    public static Dictionary<T, string> GetEnumValueDescriptionDictionary<T>() where T : struct, Enum
    {
        ValidateEnumType<T>();
        return Enum.GetValues(typeof(T))
                   .Cast<T>()
                   .ToDictionary(e => e, e => GetEnumDescription(e));
    }

    public static Dictionary<string, string> GetEnumNameDescriptionDictionary<T>() where T : struct, Enum
    {
        ValidateEnumType<T>();
        return Enum.GetValues(typeof(T))
                   .Cast<T>()
                   .ToDictionary(e => Enum.GetName(typeof(T), e) ?? string.Empty, e => GetEnumDescription(e));
    }

    public static T? GetEnumValueFromDescription<T>(string description) where T : struct, Enum
    {
        ValidateEnumType<T>();
        foreach (var field in typeof(T).GetFields())
        {
            var attribute = Attribute.GetCustomAttribute(field, typeof(System.ComponentModel.DescriptionAttribute)) as System.ComponentModel.DescriptionAttribute;
            if (attribute != null)
            {
                if (attribute.Description.Equals(description, StringComparison.OrdinalIgnoreCase))
                {
                    if (Enum.TryParse<T>(field.Name, out var result))
                    {
                        return result;
                    }
                }
            }
            else
            {
                if (field.Name.Equals(description, StringComparison.OrdinalIgnoreCase))
                {
                    if (Enum.TryParse<T>(field.Name, out var result))
                    {
                        return result;
                    }
                }
            }
        }
        return null;
    }

    public static T ParseStringAsEnum<T>(string value, bool ignoreCase = true) where T : struct, Enum
    {
        ValidateEnumType<T>();
        if (Enum.TryParse<T>(value, ignoreCase, out var result))
        {
            return result;
        }
        throw new ArgumentException($"'{value}' is not a valid value for enum '{typeof(T).Name}'");
    }

    public static T ParseDescriptionAsEnum<T>(string description) where T : struct, Enum
    {
        ValidateEnumType<T>();
        var enumValue = GetEnumValueFromDescription<T>(description);
        if (enumValue.HasValue)
        {
            return enumValue.Value;
        }
        throw new ArgumentException($"'{description}' is not a valid description for enum '{typeof(T).Name}'");
    }
}
