using System.Runtime.Serialization;

public static class EnumExtensions
{
    public static string GetEnumMemberValue(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = (EnumMemberAttribute)Attribute.GetCustomAttribute(field, typeof(EnumMemberAttribute));
        return attribute?.Value ?? value.ToString();
    }
}