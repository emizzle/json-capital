using System;
using System.Collections.Generic;

namespace JSONCapital.Common.Extensions
{
    public static class ObjectExtensions
    {
        public static bool HasDefaultValue<T>(this T value)
        {
            return EqualityComparer<T>.Default.Equals(value, default(T));
        }
    }
}
