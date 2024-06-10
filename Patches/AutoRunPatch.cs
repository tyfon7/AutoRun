using Aki.Reflection.Patching;
using EFT.InputSystem;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

using KeyHandler = GClass1897;

namespace AutoRun
{
    public class AutoRunPatch : ModulePatch
    {
        private static bool Active = false;
        private static bool CanChange = true;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(KeyHandler), nameof(KeyHandler.Update));
        }

        [PatchPrefix]
        public static bool Prefix(KeyHandler __instance, KeyCode ___Key)
        {
            bool forwardsKey = Plugin.ForwardKeys.Contains(___Key);
            bool backwardsKey = forwardsKey ? false : Plugin.BackwardKeys.Contains(___Key);

            if (!forwardsKey && !backwardsKey)
            {
                return true;
            }

            // This update method is called for every key - so it can be called multiple times in a frame depending on the number of forwards and backwards keys
            // Once it's set, don't let it change again until the keybind is not pressed
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

            __instance.Press = InputManager.UpdateInputMatrix[value, (int)__instance.Press];
            return false;
        }
    }
}
