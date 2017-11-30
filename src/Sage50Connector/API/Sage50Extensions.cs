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

        /// <summary>
        /// check PhoneNumber collection if any phone present
        /// </summary>
        /// <param name="phoneNumbers"></param>
        /// <returns></returns>
        public static bool PhonesAbsent(this List<PhoneNumber> phoneNumbers)
        {
            //Keys: honeNumber1, PhoneNumber2, Fax1
            for (int i = 1; i <= 2; i++)
            {
                var phone = phoneNumbers.SingleOrDefault(p => p.Key == $"honeNumber{i}");
                if (phone != null && !String.IsNullOrWhiteSpace(phone.Number)) return false;
            }
            return true;
        }

        /// <summary>
        /// check if one of phone number from checkWith collection exist in phoneNumbers collection
        /// </summary>
        /// <param name="phoneNumbers"></param>
        /// <param name="checkWith"></param>
        /// <returns></returns>
        public static bool ContainsOneOf(this PhoneNumberCollection phoneNumbers, List<PhoneNumber> checkWith)
        {
            //Keys: honeNumber1, PhoneNumber2, Fax1
            foreach (var phoneNumber in checkWith)
            {
                if (phoneNumbers.Any(p => p.Number == phoneNumber.Number)) return true;
            }
            return false;
        }
    }
}

