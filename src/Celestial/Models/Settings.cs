namespace Celestial.Models
{
    public class Settings
    {
        private Windows.Storage.ApplicationDataContainer LocalSettings { get; set; }

        public Settings() => LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        private void SaveSettings(string key, object value) => LocalSettings.Values[key] = value;

        private T ReadSettings<T>(string key, T defaultValue) => LocalSettings.Values.ContainsKey(key) ? (T)LocalSettings.Values[key] : null != defaultValue ? defaultValue : default;

        public static Settings Instance => new Settings();

        public bool ShowWelcomeGrid { get => ReadSettings(nameof(ShowWelcomeGrid), true); set => SaveSettings(nameof(ShowWelcomeGrid), value); }

        public bool ShowDownloadNotification { get => ReadSettings(nameof(ShowDownloadNotification), true); set => SaveSettings(nameof(ShowDownloadNotification), value); }

        public bool ShowWallpaperNotification { get => ReadSettings(nameof(ShowWallpaperNotification), true); set => SaveSettings(nameof(ShowWallpaperNotification), value); }
    }
}