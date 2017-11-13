using System;
using System.ComponentModel;
using System.Configuration;

namespace Sage50Connector.Core
{
    /// Serivce Configuration Wrapper
    static partial class ApplicationConfig
    {
        /// <summary>
        /// Get App Settings property value by key and optional default value if property does not exist
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static T GetAppSettingsValue<T>(string key, T? defaultValue = null) where T : struct
        {
            var propertyValue = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(propertyValue)) return defaultValue ?? default(T);
            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)converter.ConvertFrom(propertyValue);
        }

        /// <summary>
        /// convert datetime to UTC string format
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string DateToUTC(DateTime value)
        {
            return value.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK");
        }

        /// <summary>
        /// Set AppSettings property, store to app.config and refresh runtime ConfigurationManager
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private static void SetAppSettingsProperty(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            config.AppSettings.Settings.Remove(key);
            config.AppSettings.Settings.Add(key, value);

            // Save the configuration file.
            config.Save(ConfigurationSaveMode.Modified);

            // Force a reload of the changed section. This 
            // makes the new values available for reading.
            ConfigurationManager.RefreshSection(APP_SETTINGS_KEY);
        }
        private const string APP_SETTINGS_KEY = "appSettings";
    }
}
