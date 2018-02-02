using System;
using System.Collections.Generic;

namespace JSONCapital.Common.Extensions
{
    public static class ObjectExtensions
    {
        public static bool HasDefaultValue<T>(this T value) 
        {
            var defaultVal = GetDefaultValue(value.GetType());

            return value.Equals(defaultVal);
        }

        /// <summary>
        /// Gets the default value of type at runtime. 
        /// Taken from https://stackoverflow.com/questions/2490244/default-value-of-a-type-at-runtime
        /// There's really only two possibilities: 
        /// null for reference types and new myType() for value types (which corresponds to 0 for int, float, etc) So you really only need to account for two cases:
        /// </summary>
        /// <returns>The default value.</returns>
        /// <param name="t">Type to get default value of.</param>
        private static object GetDefaultValue(Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);

            return null;
        }
    }
}
