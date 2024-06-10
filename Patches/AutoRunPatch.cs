using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using EFT.InputSystem;
using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AutoRun
{
    public class AutoRunPatch : ModulePatch
    {
        private static bool Active = false;
        private static bool CanChange = true;

        private static Func<object, EKeyPress> PressGetter;
        private static Action<object, EKeyPress> PressSetter;

        protected override MethodBase GetTargetMethod()
        {
            Type type = PatchConstants.EftTypes.Single(t => t.GetMethod("GetKey", BindingFlags.Public | BindingFlags.Static) != null); // GClass1897

            // I'm scared about using reflection inside a movement Update() method, so using delegates for theoretically faster perf
            var pressProperty = AccessTools.Property(type, "Press");
            PressGetter = AccessTools.MethodDelegate<Func<object, EKeyPress>>(pressProperty.GetMethod, null, false);
            PressSetter = AccessTools.MethodDelegate<Action<object, EKeyPress>>(pressProperty.SetMethod, null, false);

            return AccessTools.Method(type, "Update");
        }

        [PatchPrefix]
        public static bool Prefix(object __instance, KeyCode ___Key)
        {
            bool forwardsKey = Plugin.ForwardKeys.Contains(___Key);
            bool backwardsKey = forwardsKey ? false : Plugin.BackwardKeys.Contains(___Key);

            if (!forwardsKey && !backwardsKey)
            {
                return true;
            }

            // This update method is called for every key - so it can be called multiple times in a frame depending on the number of forwards and backwards keys
            // Once it's set, we don't let it change again until the keybind is not pressed
            if (Settings.AutoRunKeyBind.Value.IsDown() && CanChange)
            {
                Active = !Active;
                CanChange = false;
            }

            bool actuallyPressed = Input.GetKey(___Key);

            // If the normal move keybind and autorun keybind overlap, you might cancel the autorun as you press it
            // So wait until the autorun key is no longer pressed before the normal key can cancel it
            if (!Settings.AutoRunKeyBind.Value.IsPressed())
            {
                CanChange = true;
                if (actuallyPressed)
                {
                    Active = false;
                }
            }

            int value;
            if (forwardsKey)
            {
                value = Active || actuallyPressed ? 1 : 0;
            }
            else
            {
                value = actuallyPressed ? 1 : 0;
            }

            PressSetter(__instance, InputManager.UpdateInputMatrix[value, (int)PressGetter(__instance)]);
            return false;
        }
    }
}
