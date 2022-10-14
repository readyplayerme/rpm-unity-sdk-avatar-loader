using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe
{
    public static class SubdomainHelper
    {
        public static string PartnerDomain;

        private static readonly bool IsLoaded;
        private static ScriptableObject partner;

        static SubdomainHelper()
        {
            if (IsLoaded) return;

            Load();
            IsLoaded = true;
        }

        public static void SaveToScriptableObject(string newSubdomain)
        {
            if (partner == null)
            {
                return;
            }
            
            var type = partner.GetType();
            var field = type.GetField("Subdomain");
            field.SetValue(partner, newSubdomain);
            SubdomainHelper.PartnerDomain = newSubdomain;
            EditorUtility.SetDirty(partner);
            AssetDatabase.SaveAssets();
        }

        private static void Load()
        {
            partner = Resources.Load<ScriptableObject>("Partner");
            var type = partner != null ? partner.GetType() : null;
            var method = type?.GetMethod("GetSubdomain");
            PartnerDomain = method?.Invoke(partner, null) as string;
        }
    }
}
