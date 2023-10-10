using System.ComponentModel;
using System.Reflection;

namespace TouristarBackend.Helpers;

public class EnumHelpers
{
    public static string GetEnumDescription(Enum value)
    {
        if (value == null) return "";
        FieldInfo? fi = value.GetType().GetField(value.ToString());
        if (fi == null) return "";
        DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (attributes.Length > 0)
            return attributes[0].Description;
        else
            return value.ToString();
    }
}

