using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace FixMod
{
    [HarmonyPatch(typeof(SceneManager), "LoadScene", new System.Type[] { typeof(int) })]
    class Level79Fix
    {
        static void Postfix(int sceneBuildIndex)
        {
            if (sceneBuildIndex != 80) return;

            var fixObject = new GameObject("Level79FixRunner");
            fixObject.AddComponent<Level79FixRunner>();
            GameObject.DontDestroyOnLoad(fixObject);
        }
    }

    class Level79FixRunner : MonoBehaviour
    {
        void Start()
        {
            StartCoroutine(FixTrigger());
        }

        IEnumerator FixTrigger()
        {
            yield return new WaitForSeconds(0.5f);

            var trigger = GameObject.Find("Trigger (1)");
            if (trigger != null)
            {
                var box = trigger.GetComponent<BoxCollider2D>();
                if (box != null)
                {
                    box.size = new Vector2(40f, box.size.y);
                }
            }

            Destroy(gameObject);
        }
    }
}