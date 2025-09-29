// ToolsMenu_FromTemplate.cs
#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using static System.IO.Path;
using static UnityEngine.Application;
#endif

namespace LH
{
    public static class FromTemplate
    {
#if UNITY_EDITOR
        private const string PACKAGE_NAME = "com.lh.tools";
        private const string SCRIPTABLEOBJECT_TEMPLATE_IN_PACKAGE = "NewScriptableObjectFile.cs.txt";

        // Menu yêu cầu: Tools -> Utilities -> Create New Scriptable Object
        [MenuItem("Tools/Utilities/Create New Scriptable Object", false, 100)]
        public static void Menu_CreateNewScriptableObject()
        {
            CreateScriptFromPackageTemplate(SCRIPTABLEOBJECT_TEMPLATE_IN_PACKAGE, "NewScriptableObject.cs");
        }

        /// <summary>
        /// Tìm template trong package và gọi ProjectWindowUtil.CreateScriptAssetFromTemplateFile để Unity mở dialog tạo file.
        /// Only package: không dò Assets hoặc global AssetDatabase find (an toàn cho package-only workflow).
        /// </summary>
        public static void CreateScriptFromPackageTemplate(string templateFileName, string defaultFileName)
        {
            string assetPath = FindTemplateAssetPathInPackage(templateFileName);
            if (string.IsNullOrEmpty(assetPath))
            {
                EditorUtility.DisplayDialog("Template not found",
                    $"Không tìm thấy template '{templateFileName}' trong package '{PACKAGE_NAME}'.\nVui lòng đặt file vào:\nPackages/{PACKAGE_NAME}/Editor/{templateFileName} (hoặc Editor/Template/...).",
                    "OK");
                Debug.LogError($"[CreateFromTemplate] Template not found in package: {templateFileName}");
                return;
            }

            Debug.Log($"[CreateFromTemplate] Using package template asset path: {assetPath}");

            // Unity sẽ tự thay #SCRIPTNAME# trong template và mở dialog đặt tên. File sẽ được tạo vào folder đang select trong Project window.
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(assetPath, defaultFileName);
        }

        /// <summary>
        /// Trả về đường dẫn asset dạng "Packages/.../Editor/<file>" (được ProjectWindowUtil chấp nhận),
        /// hoặc null nếu không tồn tại trong package.
        /// </summary>
        private static string FindTemplateAssetPathInPackage(string templateFileName)
        {
            try
            {
                string projectRoot = Path.GetFullPath(Combine(dataPath, ".."));
                string packageCandidate1 = Combine(projectRoot, "Packages", PACKAGE_NAME, "Editor", templateFileName);
                if (File.Exists(packageCandidate1))
                {
                    return $"Packages/{PACKAGE_NAME}/Editor/{templateFileName}";
                }

                string packageCandidate2 = Combine(projectRoot, "Packages", PACKAGE_NAME, "Editor", "Template", templateFileName);
                if (File.Exists(packageCandidate2))
                {
                    return $"Packages/{PACKAGE_NAME}/Editor/Template/{templateFileName}";
                }

                // not found
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[FindTemplateAssetPathInPackage] Error while checking package: {ex}");
                return null;
            }
        }

#endif
    }
}
