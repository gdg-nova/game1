namespace Apex.Editor
{
    using Apex.QuickStarts;
    using Apex.Services;
    using UnityEditor;
    using UnityEngine;

    public static class MenuExtentions
    {
        [MenuItem("GameObject/Create Other/Apex/Game World")]
        public static void GameWorldMenu()
        {
            GameObject go;

            var servicesInitializer = FindComponent<GameServicesInitializerComponent>();
            if (servicesInitializer != null)
            {
                go = servicesInitializer.gameObject;
            }
            else
            {
                go = new GameObject("Game World");
            }

            go.AddComponent<GameWorldQuickStart>();
        }

        private static T FindComponent<T>() where T : Component
        {
            var res = Resources.FindObjectsOfTypeAll<T>();

            if (res != null && res.Length > 0)
            {
                return res[0];
            }

            return null;
        }
    }
}
