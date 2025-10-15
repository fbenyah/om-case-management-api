namespace om.servicing.casemanagement.domain.Utilities;

/// <summary>
/// Provides utility methods for working with enumeration types.
/// </summary>
/// <remarks>The <see cref="EnumUtils"/> class includes methods for validating, parsing, and retrieving
/// information about enumeration types. These methods are designed to simplify common operations with enums, such as
/// validating enum values, retrieving names and descriptions, and mapping between enum values and their string
/// representations. All methods in this class require the generic type parameter <typeparamref name="T"/> to be a
/// non-nullable enumeration type.</remarks>
public static class EnumUtils
{
    /// <summary>
    /// Validates that the generic type parameter <typeparamref name="T"/> is an enumeration type.
    /// </summary>
    /// <typeparam name="T">The type to validate. Must be a value type and an enumeration.</typeparam>
    /// <exception cref="ArgumentException">Thrown if <typeparamref name="T"/> is not an enumeration type.</exception>
    public static void ValidateEnumType<T>() where T : struct, Enum
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException("T must be an enumerated type");
        }
    }

    /// <summary>
    /// Determines whether the specified string value is a valid name or numeric value of the specified enumeration
    /// type.
    /// </summary>
    /// <typeparam name="T">The enumeration type to validate against. Must be a non-nullable enum.</typeparam>
    /// <param name="value">The string representation of the enumeration value to validate.</param>
    /// <param name="ignoreCase">A boolean value indicating whether the comparison should ignore case.  <see langword="true"/> to perform a
    /// case-insensitive comparison; otherwise, <see langword="false"/>.</param>
    /// <returns><see langword="true"/> if the specified string value is a valid name or numeric value of the enumeration type
    /// <typeparamref name="T"/>;  otherwise, <see langword="false"/>.</returns>
    public static bool IsValidEnumValue<T>(string value, bool ignoreCase = true) where T : struct, Enum
    {
        ValidateEnumType<T>();
        return Enum.TryParse<T>(value, ignoreCase, out _);
    }

    /// <summary>
    /// Retrieves the names of the constants in the specified enumeration type.
    /// </summary>
    /// <typeparam name="T">The enumeration type whose constant names are to be retrieved. Must be a non-nullable enum.</typeparam>
    /// <returns>A list of strings containing the names of the constants in the enumeration type <typeparamref name="T"/>.</returns>
    public static List<string> GetEnumNames<T>() where T : struct, Enum
    {
        ValidateEnumType<T>();
        return Enum.GetNames(typeof(T)).ToList();
    }

    /// <summary>
    /// Retrieves a list of all values defined in the specified enumeration type.
    /// </summary>
    /// <remarks>This method validates that the specified type parameter <typeparamref name="T"/> is an
    /// enumeration type. If the validation fails, an <see cref="ArgumentException"/> is thrown.</remarks>
    /// <typeparam name="T">The enumeration type whose values are to be retrieved. Must be a non-nullable enum.</typeparam>
    /// <returns>A list containing all values of the enumeration type <typeparamref name="T"/>.</returns>
    public static List<T> GetEnumValues<T>() where T : struct, Enum
    {
        ValidateEnumType<T>();
        return Enum.GetValues(typeof(T)).Cast<T>().ToList();
    }

    /// <summary>
    /// Retrieves the name of the constant in the specified enumeration that has the specified value.
    /// </summary>
    /// <remarks>This method ensures that the specified type <typeparamref name="T"/> is a valid enumeration
    /// type at runtime. If the value does not correspond to any constant in the enumeration, an empty string is
    /// returned.</remarks>
    /// <typeparam name="T">The enumeration type to search. Must be a valid enumeration type.</typeparam>
    /// <param name="value">The value of the enumeration constant whose name is to be retrieved.</param>
    /// <returns>The name of the enumeration constant that has the specified value, or an empty string if no such constant is
    /// found.</returns>
    public static string GetEnumName<T>(T value) where T : struct, Enum
    {
        ValidateEnumType<T>();
        return Enum.GetName(typeof(T), value) ?? string.Empty;
    }

    /// <summary>
    /// Retrieves the enum value of type <typeparamref name="T"/> that corresponds to the specified name.
    /// </summary>
    /// <typeparam name="T">The type of the enum. Must be a struct and an enumeration.</typeparam>
    /// <param name="name">The name of the enum value to retrieve. This is case-sensitive unless <paramref name="ignoreCase"/> is set to
    /// <see langword="true"/>.</param>
    /// <param name="ignoreCase">A value indicating whether the comparison should ignore case. The default is <see langword="true"/>.</param>
    /// <returns>The enum value of type <typeparamref name="T"/> that matches the specified name, or <see langword="null"/> if no
    /// match is found.</returns>
    public static T? GetEnumValueFromName<T>(string name, bool ignoreCase = true) where T : struct, Enum
    {
        ValidateEnumType<T>();
        if (Enum.TryParse<T>(name, ignoreCase, out var result))
        {
            return result;
        }
        return null;
    }

    /// <summary>
    /// Creates a dictionary that maps the names of the constants in the specified enumeration type to their
    /// corresponding values.
    /// </summary>
    /// <remarks>The method ensures that the specified type <typeparamref name="T"/> is a valid enumeration
    /// type. If the enumeration contains duplicate names or values, the dictionary will only include the first
    /// occurrence of each name-value pair.</remarks>
    /// <typeparam name="T">The enumeration type whose names and values are to be retrieved. Must be a struct and an enumeration.</typeparam>
    /// <returns>A dictionary where the keys are the names of the constants in the enumeration type <typeparamref name="T"/>, and
    /// the values are the corresponding enumeration values.</returns>
    public static Dictionary<string, T> GetEnumNameValueDictionary<T>() where T : struct, Enum
    {
        ValidateEnumType<T>();
        return Enum.GetValues(typeof(T))
                   .Cast<T>()
                   .ToDictionary(e => Enum.GetName(typeof(T), e) ?? string.Empty, e => e);
    }

    /// <summary>
    /// Creates a dictionary that maps each value of the specified enumeration type to its name.
    /// </summary>
    /// <remarks>This method is useful for scenarios where you need a mapping of enumeration values to their 
    /// string representations, such as for display purposes or serialization. The method ensures  that the provided
    /// type parameter is a valid enumeration type before creating the dictionary.</remarks>
    /// <typeparam name="T">The enumeration type. Must be a non-nullable value type that is an enumeration.</typeparam>
    /// <returns>A dictionary where the keys are the enumeration values of type <typeparamref name="T"/>  and the values are the
    /// corresponding enumeration names as strings. If an enumeration value  does not have a name, the value will be an
    /// empty string.</returns>
    public static Dictionary<T, string> GetEnumValueNameDictionary<T>() where T : struct, Enum
    {
        ValidateEnumType<T>();
        return Enum.GetValues(typeof(T))
                   .Cast<T>()
                   .ToDictionary(e => e, e => Enum.GetName(typeof(T), e) ?? string.Empty);
    }

    /// <summary>
    /// Retrieves the description associated with an enumeration value.
    /// </summary>
    /// <typeparam name="T">The enumeration type. Must be a struct and an enumeration.</typeparam>
    /// <param name="value">The enumeration value for which to retrieve the description.</param>
    /// <returns>The description specified in the <see cref="System.ComponentModel.DescriptionAttribute"/> applied to the
    /// enumeration value, or the string representation of the value if no description is defined.</returns>
    public static string GetEnumDescription<T>(T value) where T : struct, Enum
    {
        ValidateEnumType<T>();
        var fieldInfo = typeof(T).GetField(value.ToString());
        var descriptionAttribute = fieldInfo?.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false)
                                             .FirstOrDefault() as System.ComponentModel.DescriptionAttribute;
        return descriptionAttribute?.Description ?? value.ToString();
    }

    /// <summary>
    /// Retrieves a list of descriptions for all values of the specified enumeration type.
    /// </summary>
    /// <remarks>This method uses the <see cref="System.ComponentModel.DescriptionAttribute"/> to retrieve
    /// descriptions. If an enumeration value does not have a <see cref="System.ComponentModel.DescriptionAttribute"/>,
    /// its name is returned.</remarks>
    /// <typeparam name="T">The enumeration type whose descriptions are to be retrieved. Must be a valid enumeration.</typeparam>
    /// <returns>A list of strings containing the descriptions of all enumeration values. If a value does not have a description
    /// attribute, its name is used instead.</returns>
    public static List<string> GetEnumDescriptions<T>() where T : struct, Enum
    {
        ValidateEnumType<T>();
        return Enum.GetValues(typeof(T))
                   .Cast<T>()
                   .Select(e => GetEnumDescription(e))
                   .ToList();
    }

    /// <summary>
    /// Creates a dictionary that maps each value of the specified enumeration type to its description.
    /// </summary>
    /// <remarks>This method uses the <see cref="System.ComponentModel.DescriptionAttribute"/> to retrieve 
    /// descriptions for enumeration values. If a value does not have a description attribute,  the value's name is used
    /// as the description.</remarks>
    /// <typeparam name="T">The enumeration type. Must be a non-nullable enum.</typeparam>
    /// <returns>A dictionary where the keys are the enumeration values of type <typeparamref name="T"/>  and the values are
    /// their corresponding descriptions. If an enumeration value does not have  a description attribute, its name is
    /// used as the description.</returns>
    public static Dictionary<T, string> GetEnumValueDescriptionDictionary<T>() where T : struct, Enum
    {
        ValidateEnumType<T>();
        return Enum.GetValues(typeof(T))
                   .Cast<T>()
                   .ToDictionary(e => e, e => GetEnumDescription(e));
    }

    /// <summary>
    /// Creates a dictionary that maps the names of the constants in the specified enumeration type to their
    /// corresponding descriptions.
    /// </summary>
    /// <remarks>The descriptions are retrieved using the <see
    /// cref="System.ComponentModel.DescriptionAttribute"/> applied to the enumeration constants. If no such attribute
    /// is present, the description will default to an empty string.</remarks>
    /// <typeparam name="T">The enumeration type whose names and descriptions are to be retrieved. Must be a struct and an enumeration.</typeparam>
    /// <returns>A dictionary where the keys are the names of the constants in the enumeration and the values are their
    /// corresponding descriptions. If a constant does not have a description, the value will be an empty string.</returns>
    public static Dictionary<string, string> GetEnumNameDescriptionDictionary<T>() where T : struct, Enum
    {
        ValidateEnumType<T>();
        return Enum.GetValues(typeof(T))
                   .Cast<T>()
                   .ToDictionary(e => Enum.GetName(typeof(T), e) ?? string.Empty, e => GetEnumDescription(e));
    }

    /// <summary>
    /// Retrieves an enumeration value of type <typeparamref name="T"/> that matches the specified description.
    /// </summary>
    /// <remarks>This method searches for a match by first checking the <see
    /// cref="System.ComponentModel.DescriptionAttribute"/> applied to the enumeration fields. If no match is found
    /// using the attribute, it falls back to matching the enumeration field names directly.</remarks>
    /// <typeparam name="T">The enumeration type to search. Must be a struct and an enumeration.</typeparam>
    /// <param name="description">The description or name of the enumeration value to find. The comparison is case-insensitive.</param>
    /// <returns>The enumeration value of type <typeparamref name="T"/> that matches the specified description, or <see
    /// langword="null"/> if no match is found.</returns>
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

    /// <summary>
    /// Parses a string representation of an enumeration value into the corresponding enum of type <typeparamref
    /// name="T"/>.
    /// </summary>
    /// <typeparam name="T">The enumeration type to parse. Must be a struct and an enumeration.</typeparam>
    /// <param name="value">The string representation of the enumeration value to parse.</param>
    /// <param name="ignoreCase">Specifies whether the parsing should ignore case when matching the string to an enumeration value. The default
    /// is <see langword="true"/>.</param>
    /// <returns>The enumeration value of type <typeparamref name="T"/> that corresponds to the specified string.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is not a valid name or value for the enumeration type <typeparamref
    /// name="T"/>.</exception>
    public static T ParseStringAsEnum<T>(string value, bool ignoreCase = true) where T : struct, Enum
    {
        ValidateEnumType<T>();
        if (Enum.TryParse<T>(value, ignoreCase, out var result))
        {
            return result;
        }
        throw new ArgumentException($"'{value}' is not a valid value for enum '{typeof(T).Name}'");
    }

    /// <summary>
    /// Parses the specified description and returns the corresponding value of the enum type <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>The method requires that the enum type <typeparamref name="T"/> is decorated with attributes
    /// or logic  that associates descriptions with enum values. If no matching description is found, an exception is
    /// thrown.</remarks>
    /// <typeparam name="T">The enum type to parse. Must be a valid enumeration type.</typeparam>
    /// <param name="description">The description to parse into an enum value.</param>
    /// <returns>The enum value of type <typeparamref name="T"/> that matches the specified description.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="description"/> does not match any description in the enum type <typeparamref
    /// name="T"/>.</exception>
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
