using HarmonyLib;
using UnityEngine;

namespace FixMod
{
    [HarmonyPatch(typeof(menuScr), "clkLvl48")]
    class Level48Fix
    {
        static bool Prefix(menuScr __instance)
        {
            w48scr.state = 1;
            if (MyConst.sound)
            {
                __instance.openDoorSound.Play();
            }
            return false;
        }
    }
}