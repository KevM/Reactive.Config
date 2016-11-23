using System;
using System.Reflection;

namespace Reactive.Config.Extensions
{
    public static class PropertyInfoExtensions
    {
        public static object BindValue(this PropertyInfo propertyInfo, string value)
        {
            switch (propertyInfo.Name)
            {
                case "String":
                    return value;
                case "Int32":
                    return Convert.ToInt32(value);
                case "Int64":
                    return Convert.ToInt64(value);
                case "Boolean":
                    return Convert.ToBoolean(value);
                case "DateTime":
                    return Convert.ToDateTime(value);
                default:
                    return null;
            }
        }
    }
}