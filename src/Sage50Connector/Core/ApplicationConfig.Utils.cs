using System.ComponentModel;
using System.Configuration;

namespace Sage50Connector.Core
{
    /// Service Configuration Wrapper
    static partial class ApplicationConfig
    {
        /// <summary>
        /// Get App Settings property value by key and optional default value if property does not exist
        /// </summary>
        private static T GetAppSettingsValue<T>(string key, T? defaultValue = null) where T : struct
        {
            var propertyValue = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(propertyValue)) return defaultValue ?? default(T);
            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)converter.ConvertFrom(propertyValue);
        }

        /// <summary>
        /// Set AppSettings property, store to app.config and refresh runtime ConfigurationManager
        /// </summary>
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
