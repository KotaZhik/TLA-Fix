using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;

namespace FixMod
{
    class FogResetFix
    {
        public static void Apply(Harmony harmony, bool isTLA2)
        {
            if (!isTLA2) return;

            SceneManager.sceneLoaded += (scene, mode) =>
            {
                if (scene.buildIndex != 1) return;

                var fogType = typeof(menuScr).Assembly.GetType("MainFogScr");
                if (fogType == null) return;

                var instanceField = fogType.GetField("instance", BindingFlags.Public | BindingFlags.Static);
                if (instanceField == null) return;

                var instance = instanceField.GetValue(null);
                if (instance != null)
                {
                    var setAlpha = fogType.GetMethod("SetAlpha");
                    setAlpha?.Invoke(instance, new object[] { 1f });
                }
            };
        }
    }
}