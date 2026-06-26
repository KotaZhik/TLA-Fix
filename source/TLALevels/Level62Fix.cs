using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace FixMod
{
    [HarmonyPatch(typeof(levelButtonScr), "click")]
    class Level62Fix
    {
        static bool Prefix(levelButtonScr __instance)
        {
            if (MyConst.nowLevel != 62) return true;

            var levelField = typeof(levelButtonScr).GetField("level", BindingFlags.NonPublic | BindingFlags.Instance);
            int level = (int)levelField.GetValue(__instance);
            var nField = typeof(w62scr).GetField("n", BindingFlags.Public | BindingFlags.Static);
            var openField = typeof(w62scr).GetField("open", BindingFlags.Public | BindingFlags.Static);
            int n = (int)nField.GetValue(null);

            if (level == 1 && n == 0) nField.SetValue(null, 1);
            else if (level == 12 && (n == 0 || n == 1)) nField.SetValue(null, 2);
            else if (level == 2 && n == 1) nField.SetValue(null, 2);
            else if (level == 23 && (n == 1 || n == 2)) nField.SetValue(null, 3);
            else if (level == 3 && n == 2) nField.SetValue(null, 3);
            else if (level == 34 && (n == 2 || n == 3))
            {
                nField.SetValue(null, 4);
                openField.SetValue(null, true);
                Object.FindObjectOfType<menuScr>().openDoor();
            }
            else if (level == 4 && n == 3)
            {
                nField.SetValue(null, 4);
                openField.SetValue(null, true);
                Object.FindObjectOfType<menuScr>().openDoor();
            }

            menuScr.instance.sclick.Play();
            return false;
        }
    }
}