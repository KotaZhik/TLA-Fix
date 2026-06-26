using HarmonyLib;
using UnityEngine;

namespace FixMod
{
    [HarmonyPatch(typeof(menuScr), "Awake")]
    class MenuAwakeFix
    {
        static void Postfix()
        {
            if (MenuSelecter.instance != null)
                MenuSelecter.instance.Clear();
        }
    }
}