using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace DL444.ImgurUwp.App.Settings
{
    static class AppSettingsManager
    {
        // TODO: Implement roaming.

        static ApplicationDataContainer settingsContainer = ApplicationData.Current.LocalSettings;

        public static bool KeyExists(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return settingsContainer.Values.ContainsKey(key);
        }
        public static T RetrieveEntry<T>(string key, T defaultValue = default)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var obj = settingsContainer.Values[key];
            return obj is T ? (T)obj : defaultValue;
        }
        public static void UpdateEntry(string key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            settingsContainer.Values[key] = value;
        }
        public static bool RemoveEntry(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return settingsContainer.Values.Remove(key);
        }
    }
}
