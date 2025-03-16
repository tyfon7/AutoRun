using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine;

namespace AutoRun
{
    public class Settings
    {
        // Categories
        private const string GeneralSection = "1. General";

        // General
        public static ConfigEntry<KeyboardShortcut> AutoRunKeyBind { get; set; }

        public static void Init(ConfigFile config)
        {
            var configEntries = new List<ConfigEntryBase>();

            // General
            configEntries.Add(AutoRunKeyBind = config.Bind(
                GeneralSection,
                "Auto Run",
                new KeyboardShortcut(KeyCode.W, [KeyCode.LeftControl]),
                new ConfigDescription(
                    "Keybind to auto run",
                    null,
                    new ConfigurationManagerAttributes { })));

            RecalcOrder(configEntries);
        }
        private static void RecalcOrder(List<ConfigEntryBase> configEntries)
        {
            // Set the Order field for all settings, to avoid unnecessary changes when adding new settings
            int settingOrder = configEntries.Count;
            foreach (var entry in configEntries)
            {
                if (entry.Description.Tags[0] is ConfigurationManagerAttributes attributes)
                {
                    attributes.Order = settingOrder;
                }

                settingOrder--;
            }
        }
    }
}
