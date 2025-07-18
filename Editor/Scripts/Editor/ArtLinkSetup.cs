using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace com.colorfulcoding.artlinkexporter
{
    public static class ArtLinkSetup
    {
        [MenuItem("ArtLink Exporter/Initial Setup", false, 0)]
        public static void Setup()
        {
            SetURPAsset();
        }

        private static void SetURPAsset()
        {
            string urpAssetPath = "Assets/ArtLinkExporter/Editor/Rendering/ArtLinkURPAsset.asset";
            var urpAsset = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(urpAssetPath);

            if (urpAsset == null)
            {
                Debug.Log($"URP Asset not in Assets. Looking in packages...");
                urpAssetPath = "Packages/ArtLink Exporter/Editor/Rendering/ArtLinkURPAsset.asset";
                urpAsset = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(urpAssetPath);

                if (urpAsset == null)
                {
                    Debug.LogError($"❌ URP Asset not found in packages!");
                    return;
                }
            }

            GraphicsSettings.renderPipelineAsset = urpAsset;
            QualitySettings.renderPipeline = urpAsset;

            EditorUtility.SetDirty(urpAsset);
            AssetDatabase.SaveAssets();

            Debug.Log($"URP Asset set to: {urpAsset.name}");
        }
    }
}
