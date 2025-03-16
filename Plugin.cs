using System.Collections.Generic;
using BepInEx;
using UnityEngine;

namespace AutoRun
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static List<KeyCode> ForwardKeys = [];
        public static List<KeyCode> BackwardKeys = [];

        public void Awake()
        {
            Settings.Init(Config);

            new KeyBindsPatch().Enable();
            new AutoRunPatch().Enable();
        }
    }
}
