using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace com.colorfulcoding.artlinkexporter
{
    public class SimpleAssetBundleExporterWindow : EditorWindow
    {
        private string filename;
        private bool newSelection = false;
        private Object previousSelection = null;

        [MenuItem("ArtLink Exporter/ArtLink Exporter", false, 1)]
        public static void ShowWindow()
        {
            GetWindow<SimpleAssetBundleExporterWindow>(false, "ArtLink Exporter", true);

            CheckBuildSupport();
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged()
        {
            Repaint();
        }

        private static void CheckBuildSupport()
        {
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows))
            {
                Debug.LogError("Build support for Windows is not available. Please check your Unity installation. For now, Windows exports will be skipped.");
            }else{
                Debug.Log("Build support for Windows is available.");
            }

            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android, BuildTarget.Android)){
                Debug.LogError("Build support for Android is not available. Please check your Unity installation.");
            }else{
                Debug.Log("Build support for Android is available.");
            }

            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.iOS, BuildTarget.iOS))
            {
                Debug.LogError("Build support for iOS is not available. Please check your Unity installation.");
            }else{
                Debug.Log("Build support for iOS is available.");
            }
        }

        private void OnGUI()
        {
            // Set background to pure black
            GUI.backgroundColor = Color.black;
            GUI.color = Color.white;

            // Apply the background color to the entire window
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), Color.black);

            Object selectedObject = Selection.activeObject;

            // Check if selection changed to a different prefab
            if (selectedObject != previousSelection && selectedObject != null && PrefabUtility.GetPrefabAssetType(selectedObject) != PrefabAssetType.NotAPrefab && AssetDatabase.Contains(selectedObject))
            {
                filename = selectedObject.name;
                previousSelection = selectedObject;
            }

            if (selectedObject != null && PrefabUtility.GetPrefabAssetType(selectedObject) != PrefabAssetType.NotAPrefab && AssetDatabase.Contains(selectedObject))
            {
                GUILayout.Label($"Selected Prefab: {selectedObject.name}", EditorStyles.boldLabel);

                if (newSelection)
                {
                    newSelection = false;
                    filename = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(selectedObject)).assetBundleName;
                }

                string newFilename = EditorGUILayout.TextField("AssetBundle Name", filename);
                
                // Validate filename to only allow alphanumeric characters, hyphens, and underscores
                if (newFilename != filename)
                {
                    // Remove special characters and spaces, keep only alphanumeric, hyphens, and underscores
                    string cleanFilename = System.Text.RegularExpressions.Regex.Replace(newFilename, @"[^a-zA-Z0-9\-_]", "");
                    
                    // Ensure it doesn't start with a number or hyphen
                    if (cleanFilename.Length > 0 && (char.IsDigit(cleanFilename[0]) || cleanFilename[0] == '-'))
                    {
                        cleanFilename = "asset_" + cleanFilename;
                    }
                    
                    // Ensure it's not empty
                    if (string.IsNullOrEmpty(cleanFilename))
                    {
                        cleanFilename = "asset";
                    }
                    
                    filename = cleanFilename;
                    
                    // Show warning if the input was modified
                    if (newFilename != cleanFilename)
                    {
                        EditorGUILayout.HelpBox("Special characters and spaces are not allowed. Only letters, numbers, hyphens, and underscores are permitted.", MessageType.Warning);
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                // Set Name button - black background, white text
                GUI.backgroundColor = Color.black;
                GUI.contentColor = Color.white;
                if (GUILayout.Button("Set Name", GUILayout.Width(120), GUILayout.Height(40)))
                {
                    Debug.Log($"Updating {filename}...");
                    UpdateNameAndAssetName(AssetDatabase.GetAssetPath(selectedObject), filename);
                }

                if (!string.IsNullOrEmpty(AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(selectedObject)).assetBundleName))
                {
                    // Export Assets button - black background, white text with accent
                    GUI.backgroundColor = Color.black;
                    GUI.contentColor = Color.white;
                    if (GUILayout.Button("🚀 Export Asset Bundles", GUILayout.Width(140), GUILayout.Height(40)))
                    {
                        Debug.Log($"Exporting {filename}...");
                        BuildAssetBundles(selectedObject);
                    }
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                GUI.contentColor = Color.yellow;
                GUILayout.Label($"WARNING: Please select a prefab!", EditorStyles.boldLabel);

                GUI.contentColor = Color.white;
                GUILayout.Label($"Make sure your gameobjects are saved in a prefab\nand all changes are applied. Select the prefab from file\nfor the exporter to know what to export.");
                newSelection = true;
            }
        }

        private void UpdateNameAndAssetName(string objectPath, string newName)
        {
            // Clean the name before setting it
            string cleanName = System.Text.RegularExpressions.Regex.Replace(newName, @"[^a-zA-Z0-9\-_]", "");
            
            // Ensure it doesn't start with a number or hyphen
            if (cleanName.Length > 0 && (char.IsDigit(cleanName[0]) || cleanName[0] == '-'))
            {
                cleanName = "asset_" + cleanName;
            }
            
            // Ensure it's not empty
            if (string.IsNullOrEmpty(cleanName))
            {
                cleanName = "asset";
            }
            
            AssetImporter.GetAtPath(objectPath).SetAssetBundleNameAndVariant(cleanName, "");
            AssetDatabase.RenameAsset(objectPath, cleanName);
            
            // Update the filename field to reflect the cleaned name
            filename = cleanName;
        }

        private void BuildAssetBundles(UnityEngine.Object selectedObject)
        {
            // Check mandatory build targets first
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android, BuildTarget.Android))
            {
                Debug.LogError("❌ Android build support is MANDATORY but not available. Please install Android Build Support in Unity Hub.");
                return;
            }

            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.iOS, BuildTarget.iOS))
            {
                Debug.LogError("❌ iOS build support is MANDATORY but not available. Please install iOS Build Support in Unity Hub.");
                return;
            }

            string assetBundleDirectory = "AssetBundles";

            if (!Directory.Exists(assetBundleDirectory))
            {
                Directory.CreateDirectory(assetBundleDirectory);
            }

            Debug.Log("Cleaning folder...");
            ClearFolder(assetBundleDirectory);

            Debug.Log("Starting build...");

            string assetBundleName = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(selectedObject)).assetBundleName;
            var assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);

            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = assetBundleName;
            build.assetNames = assetPaths;

            bool windowsSupported = BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
            
            if (windowsSupported)
            {
                string winPath = assetBundleDirectory + "/win";
                if (!Directory.Exists(winPath))
                {
                    Directory.CreateDirectory(winPath);
                }
                Debug.Log("Building Windows Asset Bundle...");
                BuildPipeline.BuildAssetBundles(winPath, new AssetBundleBuild[] { build }, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
            }
            else
            {
                Debug.LogWarning("⚠️ Windows build support not available. Skipping Windows Asset Bundle.");
            }

            string androidPath = assetBundleDirectory + "/android";
            if (!Directory.Exists(androidPath))
            {
                Directory.CreateDirectory(androidPath);
            }
            Debug.Log("Building Android Asset Bundle...");
            BuildPipeline.BuildAssetBundles(androidPath, new AssetBundleBuild[] { build }, BuildAssetBundleOptions.None, BuildTarget.Android);

            string iosPath = assetBundleDirectory + "/ios";
            if (!Directory.Exists(iosPath))
            {
                Directory.CreateDirectory(iosPath);
            }
            Debug.Log("Building iOS Asset Bundle...");
            BuildPipeline.BuildAssetBundles(iosPath, new AssetBundleBuild[] { build }, BuildAssetBundleOptions.None, BuildTarget.iOS);

            Debug.Log("Postprocessing...");
            CleanupAfterBuild(selectedObject, windowsSupported);
            Debug.Log("Build finished!");

            // Open the AssetBundles folder
            string bundleFolder = "AssetBundles";
            if (Directory.Exists(bundleFolder))
            {
                EditorUtility.RevealInFinder(bundleFolder);
            }
        }

        private void ClearFolder(string dirPath)
        {
            DirectoryInfo dir = new DirectoryInfo(dirPath);

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                ClearFolder(di.FullName);
                di.Delete();
            }
        }

        private void CleanupAfterBuild(UnityEngine.Object selectedObject, bool windowsSupported)
        {
            string assetBundleDirectory = "AssetBundles";
            if (!Directory.Exists(assetBundleDirectory))
            {
                Debug.LogWarning("No build present!");
                return;
            }

            string filename = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(selectedObject)).assetBundleName;

            // Handle Windows build cleanup
            if (windowsSupported)
            {
                string winPath = assetBundleDirectory + "/win";
                if (Directory.Exists(winPath))
                {
                    File.Move(winPath + "/" + filename, assetBundleDirectory + "/" + filename + "-Win");
                    Directory.Delete(winPath, true);
                }
                else
                {
                    Debug.LogWarning("Windows build not present!");
                }
            }

            // Handle Android build cleanup (mandatory)
            string androidPath = assetBundleDirectory + "/android";
            if (Directory.Exists(androidPath))
            {
                File.Move(androidPath + "/" + filename, assetBundleDirectory + "/" + filename + "-Android");
                Directory.Delete(androidPath, true);
            }
            else
            {
                Debug.LogWarning("Android build not present!");
            }

            // Handle iOS build cleanup (mandatory)
            string iosPath = assetBundleDirectory + "/ios";
            if (Directory.Exists(iosPath))
            {
                try
                {
                    File.Move(iosPath + "/" + filename, assetBundleDirectory + "/" + filename + "-IOS");
                    Directory.Delete(iosPath, true);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning("No iOS module installed?");
                    Debug.LogWarning(e);
                }
            }
            else
            {
                Debug.LogWarning("iOS build not present!");
            }
        }
    }
}