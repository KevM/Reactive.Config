using System;
using System.Reflection;

namespace Reactive.Config.Extensions
{
    public interface IPropertyValueBinder
    {
        object BindValue(PropertyInfo propertyInfo, string value);
    }

    public class PropertyValueBinder : IPropertyValueBinder
    {
        public object BindValue(PropertyInfo propertyInfo, string value)
        {
            try
            {
                switch (propertyInfo.PropertyType.Name)
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
            catch
            {
                // TODO introduce logging and add details about failed binding for diagnostics

                return null;
            }
        }
    }
}
