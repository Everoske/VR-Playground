using ShootingGallery.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootingGallery.Settings
{
    /// <summary>
    /// Service locator for Settings Manager.
    /// </summary>
    public sealed class SettingsLocator
    {
        private static SettingsManager settingsManager;

        public static void Provide(SettingsManager settings)
        {
            settingsManager = settings;
        }

        public static SettingsManager GetSettingsManager() => settingsManager;
    }
}
