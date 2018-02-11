using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace JSONCapital.Common.Extensions
{
    public static class StringExtensions
    {
        private static Dictionary<Type, Dictionary<string, object>> dicEnum = new Dictionary<Type, Dictionary<string, object>>();

        /// <summary>
        /// Parses the enum from it's string reprsentation. If parse fails, default value is returned.
        /// </summary>
        /// <returns>The enum.</returns>
        /// <param name="value">Value.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T ParseEnum<T>(this string value, T defaultValue = default(T))
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;

            var t = typeof(T);
            Dictionary<string, object> dic;
            if (!dicEnum.ContainsKey(t))
            {
                dic = new Dictionary<string, object>();
                dicEnum.Add(t, dic);
                foreach (var en in Enum.GetValues(t))
                {
                    var enumMemberVal = ((Enum)en).GetAttribute<EnumMemberAttribute>()?.Value;
                    if (!string.IsNullOrEmpty(enumMemberVal))
                    {
                        dic.Add(enumMemberVal, en);
                    }
                    dic.Add(Enum.GetName(t, en), en);
                }
            }
            else
            {
                dic = dicEnum[t];
            }

            if (!dic.ContainsKey(value))
            {
                return defaultValue;
            }
            else return (T)dic[value];
        }
    }
}
