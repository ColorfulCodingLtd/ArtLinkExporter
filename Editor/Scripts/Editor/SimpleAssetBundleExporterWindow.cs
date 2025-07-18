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

                filename = EditorGUILayout.TextField("AssetBundle Name", filename);

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
                    if (GUILayout.Button("🚀 Export Assets", GUILayout.Width(140), GUILayout.Height(40)))
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
            AssetImporter.GetAtPath(objectPath).SetAssetBundleNameAndVariant(newName, "");
            AssetDatabase.RenameAsset(objectPath, newName);
        }

        private void BuildAssetBundles(UnityEngine.Object selectedObject)
        {
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

            string winPath = assetBundleDirectory + "/win";
            if (!Directory.Exists(winPath))
            {
                Directory.CreateDirectory(winPath);
            }

            string androidPath = assetBundleDirectory + "/android";
            if (!Directory.Exists(androidPath))
            {
                Directory.CreateDirectory(androidPath);
            }

            string iosPath = assetBundleDirectory + "/ios";
            if (!Directory.Exists(iosPath))
            {
                Directory.CreateDirectory(iosPath);
            }

            BuildPipeline.BuildAssetBundles(winPath, new AssetBundleBuild[] { build }, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
            BuildPipeline.BuildAssetBundles(androidPath, new AssetBundleBuild[] { build }, BuildAssetBundleOptions.None, BuildTarget.Android);
            BuildPipeline.BuildAssetBundles(iosPath, new AssetBundleBuild[] { build }, BuildAssetBundleOptions.None, BuildTarget.iOS);

            Debug.Log("Postprocessing...");
            CleanupAfterBuild(selectedObject);
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

        private void CleanupAfterBuild(UnityEngine.Object selectedObject)
        {
            string assetBundleDirectory = "AssetBundles";
            if (!Directory.Exists(assetBundleDirectory))
            {
                Debug.LogWarning("No build present!");
                return;
            }

            string winPath = assetBundleDirectory + "/win";
            if (!Directory.Exists(winPath))
            {
                Debug.LogWarning("Windows build not present!");
            }

            string androidPath = assetBundleDirectory + "/android";
            if (!Directory.Exists(androidPath))
            {
                Debug.LogWarning("Android build not present!");
            }

            string iosPath = assetBundleDirectory + "/ios";
            if (!Directory.Exists(iosPath))
            {
                Debug.LogWarning("IOS build not present!");
            }

            string filename = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(selectedObject)).assetBundleName;

            File.Move(winPath + "/" + filename, assetBundleDirectory + "/" + filename + "-Win");
            File.Move(androidPath + "/" + filename, assetBundleDirectory + "/" + filename + "-Android");
            try
            {
                File.Move(iosPath + "/" + filename, assetBundleDirectory + "/" + filename + "-IOS");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("No iOS module installed?");
                Debug.LogWarning(e);
            }

            Directory.Delete(winPath, true);
            Directory.Delete(androidPath, true);
            Directory.Delete(iosPath, true);
        }
    }
}