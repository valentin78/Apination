using System;
using System.Linq;
using Sage.Peachtree.API;
using Sage.Peachtree.API.Collections.Generic;

namespace Sage50Connector.API
{
    internal static class Sage50Extensions
    {
        /// <summary>
        /// extension method convert string representation of enum value to appropriate enum value
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TEnum ToEnum<TEnum>(this string value) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum) throw new ArgumentException(@"Invalid generic type, must me enum", nameof(TEnum));

            if (Enum.TryParse(value, out TEnum result)) return result;

            throw new ArgumentException($"Can not convert value: '{value}' to enum type: '{typeof(TEnum)}'");
        }

        /// <summary>
        /// apply filter on EntityList by ID and load single or default item from list
        /// </summary>
        public static T SingleOrDefault<T>(this EntityList<T> list, object id) where T : Entity
        {
            return list.FilterBy("ID", id).SingleOrDefault();
        }

        /// <summary>
        /// apply filter on EntityList by propertyName it value load single or default item from list
        /// </summary>
        public static EntityList<T> FilterBy<T>(this EntityList<T> list, string propertyName, object propertyValue) where T : Entity
        {
            FilterExpression expression = FilterExpression.Equal(
                FilterExpression.Property($"{typeof(T).Name}.{propertyName}"),
                FilterExpression.Constant(propertyValue.ToString()));

            var modifier = LoadModifiers.Create();
            modifier.Filters = expression;
            list.Load(modifier);

            return list;
        }
    }
}

