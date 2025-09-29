// ToolsMenu_Folders.cs
#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using static System.IO.Path;
using static System.IO.Directory;
using static UnityEngine.Application;
#endif

namespace LH
{
    public static class ToolsMenu_CreateFolders
    {
#if UNITY_EDITOR
        private const string PACKAGE_NAME = "com.lh.tools";
        private const string SINGLETON_TEMPLATE_IN_PACKAGE = "Singleton.cs.txt";

        [MenuItem("Tools/Setup/Create Default Folders")]
        public static void CreateDefaultFolders()
        {
            Dir("_Game", "Scenes", "Scripts", "Prefabs", "ScriptableObjects", "Graphics", "Audio", "Animations", "Resources", "Sprites");
            Dir("_Game/Scripts", "Manager", "Singleton");

            // Tự động tạo Singleton.cs (silent — không ghi đè nếu đã tồn tại)
            CreateSingletonFromPackageTemplateSilent();

            AssetDatabase.Refresh();
        }

        public static void Dir(string root, params string[] dirs)
        {
            var fullPath = Combine(dataPath, root);
            foreach (var newDirectory in dirs)
            {
                CreateDirectory(Combine(fullPath, newDirectory));
            }
        }

        /// <summary>
        /// Tạo Singleton.cs bằng cách copy template trong package -> Assets/_Game/Scripts/Singleton/Singleton.cs
        /// Silent: không hỏi, không ghi đè (nếu đã tồn tại sẽ bỏ qua).
        /// </summary>
        private static void CreateSingletonFromPackageTemplateSilent()
        {
            string scriptsFolderAbsolute = Combine(dataPath, "_Game", "Scripts", "Singleton");
            string targetFileAbsolute = Combine(scriptsFolderAbsolute, "Singleton.cs");

            Debug.Log($"[CreateSingleton] target: {targetFileAbsolute}");

            // ensure folder
            try
            {
                if (!Directory.Exists(scriptsFolderAbsolute))
                {
                    Directory.CreateDirectory(scriptsFolderAbsolute);
                    Debug.Log($"[CreateSingleton] Created folder: {scriptsFolderAbsolute}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[CreateSingleton] Cannot create folder {scriptsFolderAbsolute}: {ex}");
                return;
            }

            // if exists -> skip
            if (File.Exists(targetFileAbsolute))
            {
                Debug.Log("[CreateSingleton] Singleton.cs already exists. Skipping creation.");
                return;
            }

            // find template only in package (Editor or Editor/Template)
            string projectRoot = Path.GetFullPath(Combine(dataPath, ".."));
            string packagePath1 = Combine(projectRoot, "Packages", PACKAGE_NAME, "Editor", SINGLETON_TEMPLATE_IN_PACKAGE);
            string packagePath2 = Combine(projectRoot, "Packages", PACKAGE_NAME, "Editor", "Template", SINGLETON_TEMPLATE_IN_PACKAGE);

            string found = null;
            if (File.Exists(packagePath1)) found = packagePath1;
            else if (File.Exists(packagePath2)) found = packagePath2;

            if (found == null)
            {
                Debug.LogWarning($"[CreateSingleton] Template not found in package. Checked:\n - {packagePath1}\n - {packagePath2}");
                return;
            }

            try
            {
                // copy (do not overwrite existing because we already checked)
                File.Copy(found, targetFileAbsolute, overwrite: false);
                AssetDatabase.Refresh();
                Debug.Log($"[CreateSingleton] Copied template -> {targetFileAbsolute}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[CreateSingleton] Error copying template: {ex}");
            }
        }

#endif
    }
}
