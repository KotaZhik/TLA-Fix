using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace FixMod
{
    [HarmonyPatch(typeof(pauseScr), "OnEnable")]
    class PauseButtonsReposition
    {
        static void Postfix()
        {
            GameObject pauseStuff = null;
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.name == "pauseStuff" && obj.scene.isLoaded)
                {
                    pauseStuff = obj;
                    break;
                }
            }
            if (pauseStuff == null) return;

            Transform[] allChildren = pauseStuff.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in allChildren)
            {
                if (t.name == "bSound" || t.name.ToLower().Contains("sound"))
                {
                    RectTransform soundRt = t.GetComponent<RectTransform>();
                    if (soundRt != null)
                        soundRt.anchoredPosition = new Vector2(0f, soundRt.anchoredPosition.y);
                }
                if (t.name == "bMusic" || t.name.ToLower().Contains("music"))
                {
                    RectTransform musicRt = t.GetComponent<RectTransform>();
                    if (musicRt != null)
                        musicRt.anchoredPosition = new Vector2(220f, musicRt.anchoredPosition.y);
                }
            }
        }
    }
}