using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FixMod
{
    [HarmonyPatch(typeof(CameraScr), "ChangeAllScreenState")]
    class Level43Fix
    {
        static bool Prefix()
        {
            if (SceneManager.GetActiveScene().buildIndex == 44)
                return false;
            return true;
        }
    }
}