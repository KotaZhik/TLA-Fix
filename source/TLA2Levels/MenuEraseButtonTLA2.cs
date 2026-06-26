using HarmonyLib;
using UnityEngine;

namespace FixMod
{
    [HarmonyPatch(typeof(menuScr), "clkOptions")]
    class MenuEraseButtonTLA2
    {
        static void Postfix()
        {
            string productName = Application.productName.ToLower();
            if (!productName.Contains("tla 2")) return;


            GameObject bErase = null;
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.name == "bErase" && obj.scene.isLoaded)
                {
                    bErase = obj;
                    break;
                }
            }

            if (bErase == null) return;

            bErase.SetActive(true);
        }
    }
}