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
        private static readonly string[] POOLING_TEMPLATES_IN_PACKAGE = new[]
        {
            "PoolController.cs.txt",
            "SimplePool.cs.txt",
            "GameUnit.cs.txt"
        };
        [MenuItem("Tools/Setup/Create Default Folders")]
        public static void CreateDefaultFolders()
        {
            Dir("_Game", "Scenes", "Scripts", "Prefabs", "ScriptableObjects", "Graphics", "Audio", "Animations", "Resources", "Sprites");
            Dir("_Game/Scripts", "Manager", "Singleton");
            Dir("_Game/Scripts", "Pooling");

            // Tự động tạo Singleton.cs (silent — không ghi đè nếu đã tồn tại)
            CreateSingletonFromPackageTemplateSilent();
            CreatePoolingScriptsFromPackageTemplatesSilent();
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

            // find template in package (check Editor/, Editor/Template/ and Editor/CreateDefaultTemplate/SingletonTemplate/)
            string projectRoot = Path.GetFullPath(Combine(dataPath, ".."));
            string packagePath1 = Combine(projectRoot, "Packages", PACKAGE_NAME, "Editor", SINGLETON_TEMPLATE_IN_PACKAGE);
            string packagePath2 = Combine(projectRoot, "Packages", PACKAGE_NAME, "Editor", "Template", SINGLETON_TEMPLATE_IN_PACKAGE);
            string packagePath3 = Combine(projectRoot, "Packages", PACKAGE_NAME, "Editor", "CreateDefaultTemplate", "SingletonTemplate", SINGLETON_TEMPLATE_IN_PACKAGE);

            string found = null;
            if (File.Exists(packagePath1)) found = packagePath1;
            else if (File.Exists(packagePath2)) found = packagePath2;
            else if (File.Exists(packagePath3)) found = packagePath3;

            if (found == null)
            {
                Debug.LogWarning($"[CreateSingleton] Template not found in package. Checked:\n - {packagePath1}\n - {packagePath2}\n - {packagePath3}");
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
        private static void CreatePoolingScriptsFromPackageTemplatesSilent()
        {
            string poolingFolderAbsolute = Combine(dataPath, "_Game", "Scripts", "Pooling");
            try
            {
                if (!Directory.Exists(poolingFolderAbsolute))
                {
                    Directory.CreateDirectory(poolingFolderAbsolute);
                    Debug.Log($"[CreatePooling] Created folder: {poolingFolderAbsolute}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[CreatePooling] Cannot create folder {poolingFolderAbsolute}: {ex}");
                return;
            }

            string projectRoot = Path.GetFullPath(Combine(dataPath, ".."));

            foreach (var template in POOLING_TEMPLATES_IN_PACKAGE)
            {
                // convert source template name (e.g. "SimplePool.cs.txt") to target file name ("SimplePool.cs")
                string targetName = template.EndsWith(".txt") ? template.Substring(0, template.Length - 4) : template;
                string targetFileAbsolute = Combine(poolingFolderAbsolute, targetName);

                if (File.Exists(targetFileAbsolute))
                {
                    Debug.Log($"[CreatePooling] {targetName} already exists. Skipping.");
                    continue;
                }

                string packagePath1 = Combine(projectRoot, "Packages", PACKAGE_NAME, "Editor", template);
                string packagePath2 = Combine(projectRoot, "Packages", PACKAGE_NAME, "Editor", "Template", template);
                string packagePath3 = Combine(projectRoot, "Packages", PACKAGE_NAME, "Editor", "CreateDefaultTemplate", "PoolingTemplate", template);

                string found = null;
                if (File.Exists(packagePath1)) found = packagePath1;
                else if (File.Exists(packagePath2)) found = packagePath2;
                else if (File.Exists(packagePath3)) found = packagePath3;

                if (found == null)
                {
                    Debug.LogWarning($"[CreatePooling] Template not found in package for {template}. Checked:\n - {packagePath1}\n - {packagePath2}\n - {packagePath3}");
                    continue;
                }

                try
                {
                    File.Copy(found, targetFileAbsolute, overwrite: false);
                    AssetDatabase.Refresh();
                    Debug.Log($"[CreatePooling] Copied {template} -> {targetFileAbsolute}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[CreatePooling] Error copying {template}: {ex}");
                }
            }
        }

#endif
    }
}
