using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace FixMod
{
    [HarmonyPatch(typeof(menuScr), "clkOptions")]
    class MenuEraseButtonTLA1
    {
        static void Postfix()
        {
            string productName = Application.productName.ToLower();
            if (productName.Contains("tla 2")) return;

            GameObject bErase = null;
            GameObject bLang = null;
            GameObject sure = null;

            Transform[] allChildren = GameObject.Find("options").GetComponentsInChildren<Transform>(true);
            foreach (Transform t in allChildren)
            {
                if (t.name == "bErase") bErase = t.gameObject;
                if (t.name == "bLang") bLang = t.gameObject;
                if (t.name == "sure") sure = t.gameObject;
            }

            if (bErase == null || bLang == null) return;

            Image bLangImg = bLang.GetComponent<Image>();
            Image bEraseImg = bErase.GetComponent<Image>();
            if (bLangImg != null && bEraseImg != null)
                bEraseImg.sprite = bLangImg.sprite;

            Button btn = bErase.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick = new Button.ButtonClickedEvent();
                btn.onClick.AddListener(() =>
                {
                    if (sure != null) sure.SetActive(true);
                });
            }

            if (sure != null)
                sure.transform.SetAsLastSibling();

            bErase.SetActive(true);
        }
    }
}