using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace FixMod
{
    [HarmonyPatch(typeof(pauseScr), "OnEnable")]
    class PauseExpandButton
    {
        private static bool expandViewEnabled = false;
        private static bool initialized = false;
        private static HashSet<int> camFullBlacklist = new HashSet<int> { 39, 73, 81 };

        static void Postfix()
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            bool isBlacklisted = camFullBlacklist.Contains(sceneIndex);

            if (!initialized)
            {
                expandViewEnabled = false;
                initialized = true;
            }

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

            Transform fullScreen = null;
            Transform[] allChildren = pauseStuff.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in allChildren)
            {
                if (t.name == "fullScreen") { fullScreen = t; break; }
            }
            if (fullScreen == null) return;

            GameObject oldButton = GameObject.Find("bExpandPause");
            if (oldButton != null) GameObject.Destroy(oldButton);

            GameObject expandButton = GameObject.Instantiate(fullScreen.gameObject, fullScreen.parent);
            expandButton.name = "bExpandPause";
            expandButton.SetActive(true);

            RectTransform rt = expandButton.GetComponent<RectTransform>();
            if (rt != null)
                rt.anchoredPosition = new Vector2(-220f, -110f);

            var originalBtn = expandButton.GetComponent<FullScreenBtn>();
            if (originalBtn != null) GameObject.Destroy(originalBtn);

            Text text = expandButton.GetComponentInChildren<Text>(true);
            if (text != null)
                text.text = isBlacklisted ? "Locked" : (expandViewEnabled ? "Shrink" : "Expand");

            Image imgInit = expandButton.GetComponent<Image>();
            if (imgInit != null && fullScreen != null)
            {
                var fsBtnInit = fullScreen.GetComponent<FullScreenBtn>();
                if (fsBtnInit != null)
                    imgInit.sprite = expandViewEnabled ? fsBtnInit.fs : fsBtnInit.ws;
            }

            Button btn = expandButton.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick = new Button.ButtonClickedEvent();
                btn.onClick.AddListener(() =>
                {
                    if (isBlacklisted) return;

                    expandViewEnabled = !expandViewEnabled;
                    ApplyExpandView();

                    Text txt = expandButton.GetComponentInChildren<Text>(true);
                    if (txt != null) txt.text = expandViewEnabled ? "Shrink" : "Expand";

                    Image img = expandButton.GetComponent<Image>();
                    if (img != null && fullScreen != null)
                    {
                        var fsBtn = fullScreen.GetComponent<FullScreenBtn>();
                        if (fsBtn != null)
                            img.sprite = expandViewEnabled ? fsBtn.fs : fsBtn.ws;
                    }
                });

                if (isBlacklisted) btn.interactable = false;
            }

            if (expandViewEnabled && !isBlacklisted) ApplyExpandView();
        }

        private static void ApplyExpandView()
        {
            Camera cam = Camera.main;
            if (cam != null)
                cam.orthographicSize = expandViewEnabled ? 7.5f : 11.25f;
        }

        static PauseExpandButton()
        {
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                int idx = scene.buildIndex;
                if (camFullBlacklist.Contains(idx)) return;

                if (Camera.main != null)
                    Camera.main.orthographicSize = expandViewEnabled ? 7.5f : 11.25f;
            };
        }
    }
}