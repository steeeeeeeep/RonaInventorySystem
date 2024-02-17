using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.Serialization;

namespace BikeSparesInventorySystem.Data.Enums;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());

        if (fieldInfo != null)
        {
            var displayAttribute = fieldInfo.GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute != null)
            {
                return displayAttribute.Name;
            }
        }

        return value.ToString();
    }

    public static string GetEnumValue(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());

        if (fieldInfo != null)
        {
            var enumMemberAttribute = fieldInfo.GetCustomAttribute<EnumMemberAttribute>();
            if (enumMemberAttribute != null)
            {
                return enumMemberAttribute.Value;
            }
        }

        return value.ToString();
    }
}
