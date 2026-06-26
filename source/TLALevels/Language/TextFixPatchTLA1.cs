using HarmonyLib;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using SmartLocalization;
using System.Reflection;

namespace FixMod
{
    class TextFixPatchTLA1
    {
        private static Dictionary<string, Dictionary<string, string>> textFixes = new Dictionary<string, Dictionary<string, string>>();
        private static string currentLang = "";
        private static Dictionary<string, string> currentFixes = null;

        public static void Apply(Harmony harmony)
        {
            var lmType = typeof(LanguageManager);
            var originalMethod = lmType.GetMethod("GetTextValue", new System.Type[] { typeof(string) });
            if (originalMethod == null) return;

            var postfix = typeof(TextFixPatchTLA1).GetMethod("Postfix", BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(originalMethod, postfix: new HarmonyMethod(postfix));

            LoadAllFixes();
        }

        private static void LoadAllFixes()
        {
            string gameFolder = "TLA1";
            string fixDir = Path.Combine(Application.dataPath, "..", "BepInEx", "plugins", "Fix", "TextFixes", gameFolder);
            if (!Directory.Exists(fixDir)) return;

            string[] fixFiles = Directory.GetFiles(fixDir, "*.txt");
            foreach (string file in fixFiles)
            {
                string langCode = Path.GetFileNameWithoutExtension(file).ToLower();
                var fixes = new Dictionary<string, string>();

                string[] lines = File.ReadAllLines(file);
                foreach (string line in lines)
                {
                    int eqIndex = line.IndexOf('=');
                    if (eqIndex > 0)
                    {
                        string key = line.Substring(0, eqIndex).Trim();
                        string value = line.Substring(eqIndex + 1).Trim();
                        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                            fixes[key] = value;
                    }
                }

                textFixes[langCode] = fixes;
            }
        }

        static void Postfix(ref string __result, string key)
        {
            if (string.IsNullOrEmpty(key) || __result == null) return;

            string lang = LanguageManager.Instance.CurrentlyLoadedCulture.languageCode.ToLower();

            if (currentLang != lang)
            {
                currentLang = lang;
                currentFixes = textFixes.ContainsKey(lang) ? textFixes[lang] : null;
            }

            if (currentFixes != null && currentFixes.ContainsKey(key))
            {
                __result = currentFixes[key];
            }
        }
    }
}