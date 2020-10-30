using Windows.Storage;

namespace Celestial.Helpers
{
    public class SettingsHelper : ObservableObject
    {
        private ApplicationDataContainer LocalSettings { get; set; }

        public SettingsHelper() => LocalSettings = ApplicationData.Current.LocalSettings;

        public void SaveSettings(string key, object value) => LocalSettings.Values[key] = value;

        public T ReadSettings<T>(string key, T defaultValue)
        {
            if (LocalSettings.Values.ContainsKey(key))
            {
                return (T)LocalSettings.Values[key];
            }
            if (null != defaultValue)
            {
                return defaultValue;
            }
            return default;
        }

    }
}
