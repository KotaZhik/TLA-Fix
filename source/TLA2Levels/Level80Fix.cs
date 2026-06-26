using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;

namespace FixMod
{
    class Level80Fix
    {
        public static void Apply(Harmony harmony)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex != 81) return;

            GameObject deathBarrier = new GameObject("VoidDeath");
            deathBarrier.transform.position = new Vector3(0f, -15f, 0f);

            BoxCollider2D box = deathBarrier.AddComponent<BoxCollider2D>();
            box.size = new Vector2(40f, 1f);
            box.isTrigger = true;

            var shapeScrType = typeof(menuScr).Assembly.GetType("shapeScr");
            if (shapeScrType != null)
            {
                var shapeComponent = deathBarrier.AddComponent(shapeScrType);
                var wsField = shapeScrType.GetField("ws", BindingFlags.Public | BindingFlags.Instance);
                if (wsField != null)
                {
                    var worldScrType = typeof(menuScr).Assembly.GetType("worldScr");
                    var instanceField = worldScrType?.GetField("instance", BindingFlags.Public | BindingFlags.Static);
                    var worldInstance = instanceField?.GetValue(null);
                    wsField.SetValue(shapeComponent, worldInstance);
                }

                var startMethod = shapeScrType.GetMethod("Start", BindingFlags.Public | BindingFlags.Instance);
                startMethod?.Invoke(shapeComponent, null);
            }
        }
    }
}