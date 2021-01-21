namespace Celestial.Utils
{
    public class Settings
    {
        private Windows.Storage.ApplicationDataContainer LocalSettings { get; set; }

        public Settings() => LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        private void SaveSettings(string key, object value) => LocalSettings.Values[key] = value;

        private T ReadSettings<T>(string key, T defaultValue) => LocalSettings.Values.ContainsKey(key) ? (T)LocalSettings.Values[key] : null != defaultValue ? defaultValue : default;

        public static Settings Instance => new Settings();

        public bool ShowWelcomeGrid { get => ReadSettings(nameof(ShowWelcomeGrid), true); set => SaveSettings(nameof(ShowWelcomeGrid), value); }

        public bool ShowDownloadInfo { get => ReadSettings(nameof(ShowDownloadInfo), true); set => SaveSettings(nameof(ShowDownloadInfo), value); }

        public bool ShowWallpaperInfo { get => ReadSettings(nameof(ShowWallpaperInfo), true); set => SaveSettings(nameof(ShowWallpaperInfo), value); }

        public bool SwitchToCompactPanel { get => ReadSettings(nameof(SwitchToCompactPanel), false); set => SaveSettings(nameof(SwitchToCompactPanel), value); }

        public bool IsOriginalAspectRatio { get => ReadSettings(nameof(IsOriginalAspectRatio), false); set => SaveSettings(nameof(IsOriginalAspectRatio), value); }


    }
}