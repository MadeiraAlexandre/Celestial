using Celestial.Helper;
using System;

namespace Celestial.Model
{
    public class AppSettings : SettingsHelper
    {
        public static AppSettings Instance => new AppSettings();

        public DateTimeOffset LastFeedUpdate
        {
            get => ReadSettings(nameof(LastFeedUpdate), DateTimeOffset.UtcNow);
            set
            {
                SaveSettings(nameof(LastFeedUpdate), value);
                NotifyPropertyChanged();
            }
        }

        public bool IsFirstLoad
        {
            get => ReadSettings(nameof(IsFirstLoad), true);
            set
            {
                SaveSettings(nameof(IsFirstLoad), value);
                NotifyPropertyChanged();
            }
        }

        public bool SearchUsingDatePicker
        {
            get => ReadSettings(nameof(SearchUsingDatePicker), false);
            set
            {
                SaveSettings(nameof(SearchUsingDatePicker), value);
                NotifyPropertyChanged();
            }
        }

        public bool IsFirstDownload
        {
            get => ReadSettings(nameof(IsFirstDownload), true);
            set
            {
                SaveSettings(nameof(IsFirstDownload), value);
                NotifyPropertyChanged();
            }
        }

        public bool IsFirstWallpaper
        {
            get => ReadSettings(nameof(IsFirstWallpaper), true);
            set
            {
                SaveSettings(nameof(IsFirstWallpaper), value);
                NotifyPropertyChanged();
            }
        }

    }
}
