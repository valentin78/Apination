using System;
using System.Collections.Generic;
using System.Linq;
using Sage.Peachtree.API;
using Sage.Peachtree.API.Collections.Generic;
using PhoneNumber = Sage50Connector.Models.Data.PhoneNumber;

namespace Sage50Connector.API
{
    internal static class Sage50Extensions
    {
        /// <summary>
        /// Extension method convert string representation of enum value to appropriate enum value
        /// </summary>
        public static TEnum ToEnum<TEnum>(this string value) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum) throw new ArgumentException(@"Invalid generic type, must be enum", nameof(TEnum));

            if (Enum.TryParse(value, out TEnum result)) return result;

            throw new ArgumentException($"Can not convert value: '{value}' to enum type: '{typeof(TEnum)}'");
        }

        /// <summary>
        /// Apply filter on EntityList by ID and load single or default item from list
        /// </summary>
        public static T SingleOrDefault<T>(this EntityList<T> list, object id) where T : Entity
        {
            return list.FilterBy("ID", id).SingleOrDefault();
        }

        /// <summary>
        /// Apply filter on EntityList by propertyName it value load single or default item from list
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

        /// <summary>
        /// Check if any phone presents in Sage's phoneNumbers collection
        /// </summary>
        public static bool IsPhonesAbsent(this List<PhoneNumber> phoneNumbers)
        {
            if (phoneNumbers == null)
                return false;
            // Keys for phones: PhoneNumber1, PhoneNumber2, Fax1
            return !phoneNumbers.Where(p => p.Key == "phoneNumber1" || p.Key == "phoneNumber2").Any(p => !string.IsNullOrWhiteSpace(p.Number));
        }

        /// <summary>
        /// Check if one of phone number from checkWith collection exist in Sage's phoneNumbers collection
        /// </summary>
        public static bool ContainsOneOf(this PhoneNumberCollection phoneNumbers, List<PhoneNumber> checkWith)
        {
            if (phoneNumbers == null || checkWith == null)
                return false;
            return checkWith.Any(phoneNumber => phoneNumbers.Any(p => p.Number == phoneNumber.Number));
        }
    }
}

