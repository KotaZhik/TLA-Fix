using HarmonyLib;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using SmartLocalization;

namespace FixMod
{
    class TextFixPatchTLA2
    {
        private static Dictionary<string, Dictionary<string, string>> textFixes = new Dictionary<string, Dictionary<string, string>>();
        private static string currentLang = "";
        private static Dictionary<string, string> currentFixes = null;

        public static void Apply(Harmony harmony)
        {
            string fixDir = Path.Combine(Application.dataPath, "..", "BepInEx", "plugins", "Fix", "TextFixes", "TLA2");
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

            var locScrType = typeof(menuScr).Assembly.GetType("LocScr");
            if (locScrType != null)
            {
                var getTextMethod = locScrType.GetMethod("getText", BindingFlags.Public | BindingFlags.Static);
                if (getTextMethod != null)
                {
                    var postfix = typeof(TextFixPatchTLA2).GetMethod("PostfixText", BindingFlags.Static | BindingFlags.NonPublic);
                    if (postfix != null)
                        harmony.Patch(getTextMethod, postfix: new HarmonyMethod(postfix));
                }

                var getHelpTextMethod = locScrType.GetMethod("getHelpText", BindingFlags.Public | BindingFlags.Static);
                if (getHelpTextMethod != null)
                {
                    var postfixHelp = typeof(TextFixPatchTLA2).GetMethod("PostfixHelp", BindingFlags.Static | BindingFlags.NonPublic);
                    if (postfixHelp != null)
                        harmony.Patch(getHelpTextMethod, postfix: new HarmonyMethod(postfixHelp));
                }
            }
        }

        static void UpdateCurrentFixes()
        {
            var lmType = typeof(LanguageManager);
            var instanceProp = lmType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
            var lmInstance = instanceProp?.GetValue(null);
            if (lmInstance == null) return;

            var cultureProp = lmType.GetProperty("CurrentlyLoadedCulture");
            var culture = cultureProp?.GetValue(lmInstance);
            if (culture == null) return;

            string lang = ((string)culture.GetType().GetField("languageCode")?.GetValue(culture))?.ToLower() ?? "";

            if (currentLang != lang)
            {
                currentLang = lang;
                currentFixes = textFixes.ContainsKey(lang) ? textFixes[lang] : null;
            }
        }

        static void PostfixText(ref string __result, int id)
        {
            UpdateCurrentFixes();
            string key = "text_" + id;
            if (currentFixes != null && currentFixes.ContainsKey(key))
                __result = currentFixes[key];
        }

        static void PostfixHelp(ref string __result, int id)
        {
            UpdateCurrentFixes();
            string key = "helpText_" + id;
            if (currentFixes != null && currentFixes.ContainsKey(key))
                __result = currentFixes[key];
        }
    }
}