using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Reactive.Config.Sources
{
    public class FastYetSimpleTypeActivator
    {
        private static readonly Dictionary<Type, Func<object>> Ctors = new Dictionary<Type, Func<object>>();

        public static object CreateInstance(Type type)
        {
            if (Ctors.ContainsKey(type)) return Ctors[type]();

            var exp = Expression.New(type);
            var d = Expression.Lambda<Func<object>>(exp).Compile();

            if (!Ctors.ContainsKey(type))
            {
                Ctors.Add(type, d);
            }

            return Ctors[type]();
        }
    }
}