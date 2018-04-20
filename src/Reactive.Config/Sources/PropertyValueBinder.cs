using System;
using System.Reflection;

namespace Reactive.Config.Sources
{
    /// <summary>
    /// Some configuration sources use reflection to set property values. 
    /// This binder converts the string representation of the proprety value to an object suitable for setting.
    /// Note: This will likely change when pushed harder an made more like Asp.Net MVC model binding.
    /// </summary>
    public interface IPropertyValueBinder
    {
        object BindValue(PropertyInfo propertyInfo, string value);
    }

    // TODO add unit tests

    /// <summary>
    /// Primitive binder with limited primative support. Nested objects are not supported.
    /// </summary>
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
