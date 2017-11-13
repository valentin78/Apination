using System;
using System.Configuration;

namespace Sage50Connector.Core
{
    /// Serivce Configuration Wrapper
    static class ApplicationConfig
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

        private const string APP_SETTINGS_KEY = "appSettings";
        private const string CUSTOMERS_LAST_SAVED_AT_KEY = "Customers_LastSavedAt";
        
        public static DateTime CustomersLastSavedAt
        {
            get
            {
                var p = ConfigurationManager.AppSettings[CUSTOMERS_LAST_SAVED_AT_KEY];
                if (string.IsNullOrEmpty(p)) return DateTime.MinValue;
                return DateTime.Parse(p);
            }
            set
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                config.AppSettings.Settings.Remove(CUSTOMERS_LAST_SAVED_AT_KEY);
                config.AppSettings.Settings.Add(CUSTOMERS_LAST_SAVED_AT_KEY, value.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK"));

                // Save the configuration file.
                config.Save(ConfigurationSaveMode.Modified);

                // Force a reload of the changed section. This 
                // makes the new values available for reading.
                ConfigurationManager.RefreshSection(APP_SETTINGS_KEY);
            }
        }
    }
}
