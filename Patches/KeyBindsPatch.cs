using EFT.InputSystem;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Linq;
using System.Reflection;

namespace AutoRun
{
    public class KeyBindsPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(InputManager), nameof(InputManager.UpdateBindings));
        }

        [PatchPostfix]
        public static void Postfix(AxisGroup[] settingsAxisGroups)
        {
            var axisPairs = settingsAxisGroups.Where(g => g.axisName == EAxis.MoveY).SelectMany(g => g.pairs);
            Plugin.ForwardKeys = axisPairs.SelectMany(pairs => pairs.positive.keyCode).ToList();
            Plugin.BackwardKeys = axisPairs.SelectMany(pairs => pairs.negative.keyCode).ToList();
        }
    }
}
