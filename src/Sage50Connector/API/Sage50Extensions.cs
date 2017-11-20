using System;
using System.Linq;
using Sage.Peachtree.API;
using Sage.Peachtree.API.Collections.Generic;
using Customer = Sage50Connector.Models.Payloads.Customer;
using SageCustomer = Sage.Peachtree.API.Customer;

namespace Sage50Connector.API
{
    internal static class Sage50Extensions
    {
        public static void PopulateFromModel(this SageCustomer sageCustomer, Customer customer)
        {
            sageCustomer.ID = customer.Id;
            sageCustomer.Name = customer.Name;
            sageCustomer.IsInactive = false;
            sageCustomer.AccountNumber = "";

            // set customer bill to contact properties
            sageCustomer.BillToContact.FirstName = customer.BillToContact.FirstName;
            sageCustomer.BillToContact.MiddleInitial = customer.BillToContact.MiddleInitial;
            sageCustomer.BillToContact.LastName = customer.BillToContact.LastName;
            sageCustomer.BillToContact.CompanyName = customer.BillToContact.CompanyName;
            sageCustomer.BillToContact.Address.Address1 = customer.BillToContact.Address.Address1;
            sageCustomer.BillToContact.Address.Address2 = customer.BillToContact.Address.Address2;
            sageCustomer.BillToContact.Address.City = customer.BillToContact.Address.City;
            sageCustomer.BillToContact.Address.State = customer.BillToContact.Address.State;
            sageCustomer.BillToContact.Address.Zip = customer.BillToContact.Address.Zip;
            sageCustomer.BillToContact.Address.Country = customer.BillToContact.Address.Country;

            sageCustomer.BillToContact.Gender = customer.BillToContact.Gender.ToEnum<Gender>();
        }

        public static TEnum ToEnum<TEnum>(this string value) where TEnum: struct 
        {
            if (!typeof(TEnum).IsEnum) throw new ArgumentException("Invalid generic type, must me enum", nameof(TEnum));

            if (Enum.TryParse(value, out TEnum result)) return result;

            throw new ArgumentException($"Can not convert value: '{value}' to enum type: '{typeof(TEnum)}'");
        }

        /// <summary>
        /// apply filter on EntityList by ID and load single or default item from list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="id"></param>
        public static T SingleOrDefault<T>(this EntityList<T> list, object id) where T : Entity
        {
            FilterExpression expression = FilterExpression.Equal(
                FilterExpression.Property($"{typeof(T).Name}.ID"),
                FilterExpression.Constant(id.ToString()));

            LoadModifiers modifier = LoadModifiers.Create();
            modifier.Filters = expression;
            list.Load(modifier);

            return list.SingleOrDefault();
        }
    }
}

