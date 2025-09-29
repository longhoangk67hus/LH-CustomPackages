#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using static System.IO.Directory;
using static System.IO.Path;
using static UnityEditor.AssetDatabase;
using static UnityEngine.Application;
#endif
namespace LH
{
    public static class ToolsMenu
    {
#if UNITY_EDITOR

        // --- Cấu hình (chỉnh nếu cần) ---
        private const string PACKAGE_NAME = "com.lh.tools"; // <-- đổi nếu package bạn có tên khác
        private const string SINGLETON_TEMPLATE_IN_PACKAGE = "Singleton.cs.txt";
        private const string SCRIPTABLEOBJECT_TEMPLATE_IN_PACKAGE = "NewScriptableObjectFile.cs.txt";

        [MenuItem("Tools/Setup/Create Default Folders")]
        public static void CreateDefaultFolders()
        {
            Dir("_Game", "Scenes", "Scripts", "Prefabs", "ScriptableObjects", "Graphics", "Audio", "Animations", "Resources", "Sprites");
            Dir("_Game/Scripts", "Manager", "Singleton");

            // Tự động tạo Singleton.cs (im lặng, không hỏi ghi đè)
            CreateSingletonScript();

            Refresh();
            Debug.Log("[CreateDefaultFolders] Hoàn tất: đã tạo folder mặc định và cố gắng tạo Singleton.cs.");
        }

        public static void Dir(string root, params string[] dirs)
        {
            var fullPath = Combine(dataPath, root);
            foreach (var newDirectory in dirs)
            {
                CreateDirectory(Combine(fullPath, newDirectory));
            }
        }

        #region Create Singleton Script
        /// <summary>
        /// Tạo file Singleton.cs từ template trong package.
        /// Nếu file đã tồn tại -> bỏ qua (không ghi đè).
        /// </summary>
        private static void CreateSingletonScript()
        {
            string scriptsFolderAbsolute = Combine(dataPath, "_Game", "Scripts", "Singleton");
            string targetFileAbsolute = Combine(scriptsFolderAbsolute, "Singleton.cs");
            Debug.Log($"[CreateSingletonScript] Bắt đầu. Target file: {targetFileAbsolute}");

            // Đảm bảo thư mục tồn tại
            if (!Directory.Exists(scriptsFolderAbsolute))
            {
                Directory.CreateDirectory(scriptsFolderAbsolute);
                Debug.Log($"[CreateSingletonScript] Tạo thư mục: {scriptsFolderAbsolute}");
            }

            // Nếu file đã tồn tại thì bỏ qua
            if (File.Exists(targetFileAbsolute))
            {
                Debug.Log("[CreateSingletonScript] File Singleton.cs đã tồn tại -> bỏ qua.");
                return;
            }

            // Tìm template **chỉ trong package**
            string foundTemplate = FindTemplateInPackage(SINGLETON_TEMPLATE_IN_PACKAGE);
            if (foundTemplate == null)
            {
                Debug.LogWarning($"[CreateSingletonScript] KHÔNG tìm thấy template '{SINGLETON_TEMPLATE_IN_PACKAGE}' trong package '{PACKAGE_NAME}'. Vui lòng đặt file vào: Packages/{PACKAGE_NAME}/Editor/{SINGLETON_TEMPLATE_IN_PACKAGE}");
                return;
            }

            try
            {
                File.Copy(foundTemplate, targetFileAbsolute, false);
                Refresh();
                Debug.Log($"[CreateSingletonScript] Đã tạo Singleton.cs từ template: {foundTemplate}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[CreateSingletonScript] Lỗi khi copy template: {ex}");
            }
        }
        #endregion

        #region Create NewScriptableObject Script
        // Menu item để tạo ScriptableObject từ template trong package
        [MenuItem("Tools/Utilities/Create New Scriptable Object", false, 100)]
        public static void CreateScriptableObjectFromTemplateMenu()
        {
            const string defaultFileName = "NewScriptableObject.cs";

            string templateAssetPath = FindTemplateAssetPathInPackage(SCRIPTABLEOBJECT_TEMPLATE_IN_PACKAGE);
            if (string.IsNullOrEmpty(templateAssetPath))
            {
                EditorUtility.DisplayDialog("Template not found",
                    $"Không tìm thấy template \"{SCRIPTABLEOBJECT_TEMPLATE_IN_PACKAGE}\" trong package '{PACKAGE_NAME}'.\n\nVui lòng đặt template vào:\nPackages/{PACKAGE_NAME}/Editor/{SCRIPTABLEOBJECT_TEMPLATE_IN_PACKAGE}",
                    "OK");
                Debug.LogError($"[CreateScriptableObjectFromTemplateMenu] KHÔNG tìm thấy template: {SCRIPTABLEOBJECT_TEMPLATE_IN_PACKAGE} trong package {PACKAGE_NAME}");
                return;
            }

            Debug.Log($"[CreateScriptableObjectFromTemplateMenu] Sử dụng template (package): {templateAssetPath}");

            // Gọi Unity để mở dialog tạo script và thay #SCRIPTNAME# tự động
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templateAssetPath, defaultFileName);
        }

        /// <summary>
        /// Trả về đường dẫn asset dạng "Packages/..." nếu template file tồn tại trong package.
        /// Trả về null nếu không tìm thấy.
        /// </summary>
        private static string FindTemplateAssetPathInPackage(string templateFileName)
        {
            try
            {
                string projectRoot = Path.GetFullPath(Path.Combine(dataPath, ".."));
                string packageCandidateFs = Path.Combine(projectRoot, "Packages", PACKAGE_NAME, "Editor", templateFileName);

                if (File.Exists(packageCandidateFs))
                {
                    // Unity chấp nhận "Packages/<packageName>/..." làm asset path cho file trong package
                    return $"Packages/{PACKAGE_NAME}/Editor/{templateFileName}";
                }

                // Nếu template nằm trong Editor/Template/
                string packageCandidateFsAlt = Path.Combine(projectRoot, "Packages", PACKAGE_NAME, "Editor", "Template", templateFileName);
                if (File.Exists(packageCandidateFsAlt))
                {
                    return $"Packages/{PACKAGE_NAME}/Editor/Template/{templateFileName}";
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[FindTemplateAssetPathInPackage] Lỗi khi kiểm tra package template: {ex}");
                return null;
            }
        }

        /// <summary>
        /// Trả về đường dẫn file hệ thống (absolute) nếu tồn tại trong package (dùng cho copy).
        /// </summary>
        private static string FindTemplateInPackage(string templateFileName)
        {
            try
            {
                string projectRoot = Path.GetFullPath(Path.Combine(dataPath, ".."));
                string packageCandidateFs = Path.Combine(projectRoot, "Packages", PACKAGE_NAME, "Editor", templateFileName);

                if (File.Exists(packageCandidateFs))
                {
                    return packageCandidateFs;
                }

                string packageCandidateFsAlt = Path.Combine(projectRoot, "Packages", PACKAGE_NAME, "Editor", "Template", templateFileName);
                if (File.Exists(packageCandidateFsAlt))
                {
                    return packageCandidateFsAlt;
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[FindTemplateInPackage] Lỗi khi kiểm tra package filesystem: {ex}");
                return null;
            }
        }
        #endregion

#endif
    }
}
