using UnityEditor;
using static System.IO.Directory;
using static System.IO.Path;
using static UnityEditor.AssetDatabase;
using static UnityEngine.Application;

namespace LH
{
    public static class ToolsMenu
    {
        [MenuItem("Tools/Setup/Create Default Folders")]
        public static void CreateDefaultFolders()
        {
            Dir("_Game", "Scenes", "Scripts", "Prefabs", "ScriptableObjects", "Graphics", "Audio", "Animations", "Resources", "Sprites");
            Dir("Scripts", "Singleton", "Manager");
            Refresh();
        }
        public static void Dir(string root, params string[] dirs)
        {
            var fullPath = Combine(dataPath, root);
            foreach (var newDirectory in dirs)
            {
                CreateDirectory(Combine(fullPath, newDirectory));
            }
        }
        private static void CreateSingletonScript()
        {
            string scriptsPath = Combine(dataPath, "Scripts", "Singleton");
            string targetFile = Combine(scriptsPath, "Singleton.cs");
            string templatePath = Combine("Packages", "LH.CreateDefaultDirectory", "Editor", "Singleton.cs.txt");

            if (!File.Exists(targetFile) && File.Exists(templatePath))
            {
                File.Copy(templatePath, targetFile);
            }
        }
    }
    
}
