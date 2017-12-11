using System;
using System.Collections.Generic;
using System.Configuration;

namespace Sage50Connector.Core
{
    /// Serivce Configuration Wrapper
    static partial class ApplicationConfig
    {
        /// <summary>
        /// A valid application ID is needed in order to connect to regular Sage 50 companies
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static string Sage50ApplicationID => ConfigurationManager.AppSettings["Sage50ApplicationID"];

        /// <summary>
        /// Authenticate token for authorization purpose
        /// </summary>
        public static string AuthToken => ConfigurationManager.AppSettings["AuthToken"];

        /// <summary>
        /// Apination REST Api Base Url
        /// </summary>
        public static Uri ApinationBaseUri => new Uri(ConfigurationManager.AppSettings["ApinationBaseUrl"]);

        /// <summary>
        /// Configuration endpoint relative Url
        /// </summary>
        public static string ConfigRelativeUrl => ConfigurationManager.AppSettings["ConfigRelativeUrl"];

        /// <summary>
        /// Apination ClientId
        /// </summary>
        public static string ClientId => ConfigurationManager.AppSettings["ClientId"];

        /// <summary>
        /// Apination Log Url
        /// </summary>
        public static string ApinationLogUrl => ConfigurationManager.AppSettings["ApinationLogUrl"];

        public static List<Company> CompaniesList => ConfigurationManager.GetSection("companiesList") as List<Company>;

        /// <summary>
        /// Customers LastSavedAt value filter
        /// </summary>
        public static DateTime CustomersLastSavedAt
        {
            get => GetAppSettingsValue<DateTime>(CustomersLastSavedAtKey);
            set => SetAppSettingsProperty(CustomersLastSavedAtKey, TypeUtil.DateToUTC(value));
        }

        private const string CustomersLastSavedAtKey = "Customers_LastSavedAt";
    }
}
