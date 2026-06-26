using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace FixMod
{
    [BepInPlugin("com.KotaZhik.Fix", "Fix", "1.0")]
    public class Fix : BaseUnityPlugin
    {
        private void Awake()
        {
            Harmony harmony = new Harmony("com.KotaZhik.Fix");

            string productName = Application.productName.ToLower();
            bool isTLA2 = productName.Contains("tla 2");

            if (!isTLA2)
            {
                harmony.PatchAll(typeof(PauseButtonsReposition));
                harmony.PatchAll(typeof(PauseExpandButton));
                harmony.PatchAll(typeof(MenuEraseButtonTLA1));
                harmony.PatchAll(typeof(MenuAwakeFix));
                harmony.PatchAll(typeof(Level62Fix));
                harmony.PatchAll(typeof(Level48Fix));
                TextFixPatchTLA1.Apply(harmony);
            }
            else
            {
                harmony.PatchAll(typeof(MenuEraseButtonTLA2));
                harmony.PatchAll(typeof(Level43Fix));
                harmony.PatchAll(typeof(Level79Fix));
                Level80Fix.Apply(harmony);
                FogResetFix.Apply(harmony, isTLA2);
                TextFixPatchTLA2.Apply(harmony);
            }

            Logger.LogInfo($"Fix loaded for {(isTLA2 ? "TLA2" : "TLA1")}!");
        }
    }
}
