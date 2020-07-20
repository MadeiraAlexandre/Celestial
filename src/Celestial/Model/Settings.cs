using Celestial.Helper;
using System;

namespace Celestial.Model
{
    public class Settings : SettingsHelper
    {
        public static Settings Instance => new Settings();

        public DateTimeOffset LastFeedUpdate
        {
            get => ReadSettings(nameof(LastFeedUpdate), DateTimeOffset.UtcNow);
            set
            {
                SaveSettings(nameof(LastFeedUpdate), value);
                NotifyPropertyChanged();
            }
        }

        public int NumberOfObjects
        {
            get => ReadSettings(nameof(NumberOfObjects), 0);
            set
            {
                SaveSettings(nameof(NumberOfObjects), value);
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


    }
}
