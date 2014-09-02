namespace Apex.Editor
{
    using System;
    using System.Text;
    using UnityEditor;
    using UnityEngine;

    public static class EditorUtilities
    {
        public static string SplitToWords(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            var transformer = new StringBuilder();

            transformer.Append(char.ToUpper(s[0]));
            for (int i = 1; i < s.Length; i++)
            {
                if (char.IsUpper(s, i))
                {
                    transformer.Append(" ");
                }

                transformer.Append(s[i]);
            }

            return transformer.ToString();
        }

        public static void CreateOrUpdateAsset<T>(T obj, string assetName = "", string defaultAssetSubPath = "") where T : ScriptableObject
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (AssetDatabase.Contains(obj))
            {
                EditorUtility.SetDirty(obj);
            }
            else
            {
                //Have to do this rather cumbersome path construction due to the workings of the AssetDatabase methods
                path = "Assets";

                var subPath = GetSubPath(defaultAssetSubPath);
                if (!string.IsNullOrEmpty(subPath))
                {
                    path = string.Concat(path, "/", subPath);
                }

                var folderId = AssetDatabase.AssetPathToGUID(path);
                if (string.IsNullOrEmpty(folderId))
                {
                    folderId = AssetDatabase.CreateFolder("Assets", subPath);
                    path = AssetDatabase.GUIDToAssetPath(folderId);
                }

                if (string.IsNullOrEmpty(assetName))
                {
                    assetName = typeof(T).Name;
                }

                path = string.Concat(path, "/", assetName, ".asset");
                path = AssetDatabase.GenerateUniqueAssetPath(path);

                AssetDatabase.CreateAsset(obj, path);

                EditorGUIUtility.PingObject(obj);
            }

            AssetDatabase.SaveAssets();
        }

        public static void RemoveAsset<T>(T obj) where T : ScriptableObject
        {
            if (AssetDatabase.Contains(obj))
            {
                string path = AssetDatabase.GetAssetPath(obj);
                AssetDatabase.DeleteAsset(path);
            }
        }

        private static string GetSubPath(string path)
        {
            if (string.IsNullOrEmpty(path) || path.Equals("Assets", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            return path.Trim('/', '\\');
        }
    }
}
