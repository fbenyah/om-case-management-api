using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace om.servicing.casemanagement.domain.Converters;

public class EnumDescriptionJsonConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var description = reader.GetString();
        foreach (var value in Enum.GetValues(typeof(TEnum)))
        {
            var field = typeof(TEnum).GetField(value.ToString());
            var attr = field?.GetCustomAttribute<DescriptionAttribute>();
            if (attr != null && attr.Description == description)
                return (TEnum)value;
        }
        throw new JsonException($"Unknown description '{description}' for {typeof(TEnum).Name}.");
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        var field = typeof(TEnum).GetField(value.ToString());
        var attr = field?.GetCustomAttribute<DescriptionAttribute>();
        writer.WriteStringValue(attr?.Description ?? value.ToString());
    }
}
