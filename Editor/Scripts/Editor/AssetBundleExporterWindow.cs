using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace com.colorfulcoding.artlinkexporter
{
    public class AssetBundleExporterWindow : EditorWindow
    {
        private string filename;
        private bool newSelection = false;
        private List<UnityEngine.Object> selectedObjects = new List<UnityEngine.Object>();

        [MenuItem("ArtLink Exporter/AssetBundle Exporter")]
        public static void ShowWindow()
        {
            GetWindow<AssetBundleExporterWindow>(false, "AB Exporter", true);
        }

        private void OnGUI()
        {
            GUILayout.Label("PREFAB EXPORTER");

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            Object selectedObject = Selection.activeObject;
            if (selectedObject != null && PrefabUtility.GetPrefabAssetType(selectedObject) != PrefabAssetType.NotAPrefab && AssetDatabase.Contains(selectedObject))
            {
                string prefabName = Selection.activeObject.name;
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

                if (GUILayout.Button("Set Name", GUILayout.Width(120), GUILayout.Height(40)))
                {
                    Debug.Log($"Updating {filename}...");
                    UpdateNameAndAssetName(AssetDatabase.GetAssetPath(selectedObject), filename);
                }

                if (!string.IsNullOrEmpty(AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(selectedObject)).assetBundleName))
                {
                    GUI.backgroundColor = Color.green;
                    if (GUILayout.Button("+ Add to export", GUILayout.Width(120), GUILayout.Height(40)))
                    {
                        Debug.Log($"Adding {filename}...");
                        if (!selectedObjects.Contains(selectedObject))
                        {
                            selectedObjects.Add(selectedObject);
                        }
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

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GuiLine();

            ShowSelectedObjects();

            GuiLine();

            Documentation();
        }

        public void Documentation()
        {
            GUILayout.Label("NOTES");
            GUILayout.Label("> Make sure parent object is at Position zero, rotation zero and scale one.");
            GUILayout.Label("> Set \"Color Settings\" from Gamma to Linear, in Project Settings.");
            GUILayout.Label("> For matching rendering settings (e.g. for using portals) use the URP Render Asset in /Rendering/");
        }

        private void ShowSelectedObjects()
        {
            GUILayout.Label("List of ready to export bundles: ", EditorStyles.boldLabel);
            if (selectedObjects.Count == 0)
            {
                GUILayout.Label("--No objects added to export list--");
            }
            else
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                foreach (Object obj in selectedObjects)
                {
                    GUILayout.Label("> " + obj.name);
                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("X Clear All", GUILayout.Width(120), GUILayout.Height(40)))
                {
                    selectedObjects.Clear();
                }

                GUI.backgroundColor = Color.blue;
                if (GUILayout.Button("Export All", GUILayout.Width(150), GUILayout.Height(40)))
                {
                    Debug.Log($"Exporting {filename}...");
                    BuildAssetBundles(selectedObjects);
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }

        private void UpdateNameAndAssetName(string objectPath, string newName)
        {
            AssetImporter.GetAtPath(objectPath).SetAssetBundleNameAndVariant(newName, "");
            AssetDatabase.RenameAsset(objectPath, newName);
        }

        private void GuiLine(int i_height = 1)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, i_height);
            rect.height = i_height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }

        private void BuildAssetBundles(List<UnityEngine.Object> objects)
        {
            string assetBundleDirectory = "AssetBundles";

            if (!Directory.Exists(assetBundleDirectory))
            {
                Directory.CreateDirectory(assetBundleDirectory);
            }

            Debug.Log("Cleaning folder...");

            ClearFolder(assetBundleDirectory);

            Debug.Log("Starting build...");

            List<AssetBundleBuild> builds = new List<AssetBundleBuild>();

            foreach (var selectedObject in objects)
            {
                string assetBundleName = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(selectedObject)).assetBundleName;
                var assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);

                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = assetBundleName;
                build.assetNames = assetPaths;

                builds.Add(build);
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

                BuildPipeline.BuildAssetBundles(winPath, builds.ToArray(), BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
                BuildPipeline.BuildAssetBundles(androidPath, builds.ToArray(), BuildAssetBundleOptions.None, BuildTarget.Android);
                BuildPipeline.BuildAssetBundles(iosPath, builds.ToArray(), BuildAssetBundleOptions.None, BuildTarget.iOS);

            }
            Debug.Log("Postprocessing...");
            CleanupAfterBuild(selectedObjects);
            Debug.Log("Build finished!");
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


        //[MenuItem("Assets/Cleanup After Build")]
        private void CleanupAfterBuild(List<UnityEngine.Object> selectedObjects)
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

            foreach (Object obj in selectedObjects)
            {
                string filename = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(obj)).assetBundleName;

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
            }

            Directory.Delete(winPath, true);
            Directory.Delete(androidPath, true);
            Directory.Delete(iosPath, true);
        }
    }
}