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
        // Tên package đúng với repo của bạn
        private const string PACKAGE_NAME = "com.lh.tools";
        private const string SCRIPTABLEOBJECT_TEMPLATE_IN_PACKAGE = "NewScriptableObjectFile.cs.txt";

        // Menu: Tools -> Utilities -> Create New Scriptable Object
        [MenuItem("Tools/Utilities/Create New Scriptable Object", false, 100)]
        public static void Menu_CreateNewScriptableObject()
        {
            CreateScriptFromPackageTemplate(SCRIPTABLEOBJECT_TEMPLATE_IN_PACKAGE, "NewScriptableObject.cs");
        }

        /// <summary>
        /// Lấy template từ package và gọi ProjectWindowUtil để Unity mở dialog và thay #SCRIPTNAME#
        /// </summary>
        public static void CreateScriptFromPackageTemplate(string templateFileName, string defaultFileName)
        {
            string assetPath = FindTemplateAssetPathInPackage(templateFileName);
            if (string.IsNullOrEmpty(assetPath))
            {
                EditorUtility.DisplayDialog("Template not found",
                    $"Không tìm thấy template '{templateFileName}' trong package '{PACKAGE_NAME}'.\nVui lòng đặt file vào:\nPackages/{PACKAGE_NAME}/Editor/Template/{templateFileName}",
                    "OK");
                Debug.LogError($"[CreateFromTemplate] Template not found: {templateFileName}");
                return;
            }

            Debug.Log($"[CreateFromTemplate] Using template: {assetPath}");
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(assetPath, defaultFileName);
        }

        /// <summary>
        /// Trả về asset path dạng "Packages/.../Editor/Template/<file>" nếu tồn tại
        /// </summary>
        private static string FindTemplateAssetPathInPackage(string templateFileName)
        {
            try
            {
                string projectRoot = Path.GetFullPath(Combine(dataPath, ".."));
                string candidate = Combine(projectRoot, "Packages", PACKAGE_NAME, "Editor", "Template", templateFileName);
                if (File.Exists(candidate))
                {
                    return $"Packages/{PACKAGE_NAME}/Editor/Template/{templateFileName}";
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[FindTemplateAssetPathInPackage] Error: {ex}");
                return null;
            }
        }

#endif
    }
}
