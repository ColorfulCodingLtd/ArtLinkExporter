using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;

namespace com.colorfulcoding.artlinkexporter
{
    public interface ISetting { }

    [System.Serializable]
    public class GeneralSettings : ISetting
    {
        [SerializeField]
        public string name = "general-settings";
        [SerializeField]
        public string disableDefaultLight = "false";
        [SerializeField]
        public string hasReticle = "false";
    }

    [Serializable]
    public class CustomThumbnailSetting : ISetting
    {
        [SerializeField]
        public string customThumbnail;
        [SerializeField]
        public bool isCover;
    }

    [System.Serializable]
    public class SetContentOnClickSetting : ISetting
    {
        [SerializeField]
        public string name;
        [SerializeField]
        public string target;
        [SerializeField]
        public string url;
        //sound|image
        [SerializeField]
        public string type;
    }

    [System.Serializable]
    public class HideShowOnClickSetting : ISetting
    {
        [SerializeField]
        public string name;
        [SerializeField]
        public List<string> hide;
        [SerializeField]
        public List<string> show;

        public HideShowOnClickSetting()
        {
            hide = new List<string>();
            show = new List<string>();
        }
    }

    [System.Serializable]
    public class OpenUrlSetting : ISetting
    {
        [SerializeField]
        public string name;
        [SerializeField]
        public string openUrl;
    }

    [System.Serializable]
    public class VideoclipFromUrlSetting : ISetting
    {
        [SerializeField]
        public string name;
        [SerializeField]
        public string videoclipUrl;
        [SerializeField]
        public bool loop = true;
    }

    [System.Serializable]
    public class TextureFromUrlSetting : ISetting
    {
        [SerializeField]
        public string name;
        [SerializeField]
        public string imageUrl;
    }

    [System.Serializable]
    public class FrameActionSetting
    {
        public double videoTimeInSeconds;
        public string paramType;
        public string animationControllerParamName;
        public int animationControllerParamValueNumber;
        public bool animationControllerParamValueBool;
    }

    [System.Serializable]
    public class VideoClipControlsAnimationSetting : ISetting
    {
        [SerializeField]
        public string name;
        [SerializeField]
        public string videoPlayer;
        [SerializeField]
        public List<FrameActionSetting> frameActions;

        public VideoClipControlsAnimationSetting()
        {
            frameActions = new List<FrameActionSetting>();
        }

    }


    [System.Serializable]
    public class ReticleNearbyInfluenceBlendTreeBody : ISetting
    {
        [SerializeField]
        public string paramName;
        [SerializeField]
        public float screenWidthRatioMax;
        [SerializeField]
        public float screenWidthRatioMin;
        [SerializeField]
        public int zDistance;
    }

    [System.Serializable]
    public class ReticleClickBody : ISetting
    {
        [SerializeField]
        public string paramName;
        [SerializeField]
        public string openUrl;
    }

    [System.Serializable]
    public class ReticleNearbyInfluenceBlendTree : ISetting
    {
        [SerializeField]
        public string name;
        [SerializeField]
        public ReticleNearbyInfluenceBlendTreeBody reticleNearByBlendTree;

        public ReticleNearbyInfluenceBlendTree()
        {
            reticleNearByBlendTree = new ReticleNearbyInfluenceBlendTreeBody();
        }

    }

    [System.Serializable]
    public class ReticleClickBehaviour : ISetting
    {
        [SerializeField]
        public string name;
        [SerializeField]
        public ReticleClickBody reticleClick;

        public ReticleClickBehaviour()
        {
            reticleClick = new ReticleClickBody();
        }

    }

    [System.Serializable]
    public class CustomPositionSetting : ISetting
    {
        [SerializeField]
        public string name;
        [SerializeField]
        public float[] position;

        public CustomPositionSetting()
        {
            position = new float[3] { 0, 0, 0 };
        }
    }

    [System.Serializable]
    public class CustomScaleSetting : ISetting
    {
        [SerializeField]
        public string name;
        [SerializeField]
        public float[] scale;

        public CustomScaleSetting()
        {
            scale = new float[3] { 0, 0, 0 };
        }
    }

    [System.Serializable]
    public class LoadRandomImageOnTapSetting : ISetting
    {
        [SerializeField]
        public string name = "Target";
        [SerializeField]
        public string loadRandomImageOnTap = "https://artlink.s3.eu-central-1.amazonaws.com/2025/destructura-2025/collages/*.png";


    }


    public class CustomBehaviourGenerator
    {
        List<ISetting> settings = new List<ISetting>();

        private string customBehavioursLoadJson = "Load json";
        private Vector2 pos;

        public void OnGUI()
        {
            GUI.backgroundColor = Color.gray;
            pos = EditorGUILayout.BeginScrollView(pos);

            UnityEngine.Object selectedObject = Selection.activeObject;

            GUILayout.Label("CUSTOM BEHAVIOUR GENERATOR");

            if (selectedObject != null)
            {
                GUILayout.Label($"SELECTION: {selectedObject.name}");
            }
            else
            {
                GUILayout.Label("No selection!");
            }

            GUILayout.Label(Serialize(settings));

            GuiLine();

            foreach (ISetting setting in settings)
            {
                EditorGUILayout.Space();
                if (setting is GeneralSettings genSetting)
                {
                    genSetting.disableDefaultLight = EditorGUILayout.TextField("DisableDefaultLight true|false", genSetting.disableDefaultLight);
                    genSetting.hasReticle = EditorGUILayout.TextField("Has Reticle true | false", genSetting.hasReticle);
                }
                else if (setting is CustomThumbnailSetting customThumbnail)
                {
                    customThumbnail.customThumbnail = EditorGUILayout.TextField("Custom Thumbnail URL", customThumbnail.customThumbnail);
                    customThumbnail.isCover = EditorGUILayout.Toggle("Is Cover", customThumbnail.isCover);
                }
                else if (setting is SetContentOnClickSetting setContentSetting)
                {
                    EditorGUILayout.BeginHorizontal();
                    setContentSetting.name = EditorGUILayout.TextField("Name", setContentSetting.name);

                    if (GUILayout.Button("SetSelectedObjectPath", GUILayout.Width(180), GUILayout.Height(20)))
                    {
                        setContentSetting.name = GetCompletePath(Selection.activeGameObject);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    setContentSetting.target = EditorGUILayout.TextField("Target", setContentSetting.target);

                    if (GUILayout.Button("SetSelectedObjectPath", GUILayout.Width(180), GUILayout.Height(20)))
                    {
                        setContentSetting.target = GetCompletePath(Selection.activeGameObject);
                    }
                    EditorGUILayout.EndHorizontal();

                    setContentSetting.url = EditorGUILayout.TextField("URL", setContentSetting.url);
                    setContentSetting.type = EditorGUILayout.TextField("Type sound|image", setContentSetting.type);
                }
                else if (setting is HideShowOnClickSetting hideShowSetting)
                {
                    EditorGUILayout.BeginHorizontal();
                    hideShowSetting.name = EditorGUILayout.TextField("Name", hideShowSetting.name);

                    if (GUILayout.Button("SetSelectedObjectPath", GUILayout.Width(180), GUILayout.Height(20)))
                    {
                        hideShowSetting.name = GetCompletePath(Selection.activeGameObject);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Hide");

                    if (GUILayout.Button("Add", GUILayout.Width(180), GUILayout.Height(20)))
                    {
                        hideShowSetting.hide.Add("");
                    }
                    if (GUILayout.Button("Remove", GUILayout.Width(180), GUILayout.Height(20)))
                    {
                        hideShowSetting.hide.RemoveAt(hideShowSetting.hide.Count - 1);
                    }
                    EditorGUILayout.EndHorizontal();

                    for (int i = 0; i < hideShowSetting.hide.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        hideShowSetting.hide[i] = EditorGUILayout.TextField("Hide", hideShowSetting.hide[i]);

                        if (GUILayout.Button("SetSelectedObjectPath", GUILayout.Width(180), GUILayout.Height(20)))
                        {
                            hideShowSetting.hide[i] = GetCompletePath(Selection.activeGameObject);
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Show");

                    if (GUILayout.Button("Add", GUILayout.Width(180), GUILayout.Height(20)))
                    {
                        hideShowSetting.show.Add("");
                    }
                    if (GUILayout.Button("Remove", GUILayout.Width(180), GUILayout.Height(20)))
                    {
                        hideShowSetting.show.RemoveAt(hideShowSetting.show.Count - 1);
                    }
                    EditorGUILayout.EndHorizontal();

                    for (int i = 0; i < hideShowSetting.show.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        hideShowSetting.show[i] = EditorGUILayout.TextField("Hide", hideShowSetting.show[i]);

                        if (GUILayout.Button("SetSelectedObjectPath", GUILayout.Width(180), GUILayout.Height(20)))
                        {
                            hideShowSetting.show[i] = GetCompletePath(Selection.activeGameObject);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                else if (setting is OpenUrlSetting openUrlSetting)
                {
                    EditorGUILayout.BeginHorizontal();
                    openUrlSetting.name = EditorGUILayout.TextField("Name", openUrlSetting.name);

                    if (GUILayout.Button("SetSelectedObjectPath", GUILayout.Width(180), GUILayout.Height(20)))
                    {
                        openUrlSetting.name = GetCompletePath(Selection.activeGameObject);
                    }
                    EditorGUILayout.EndHorizontal();
                    openUrlSetting.openUrl = EditorGUILayout.TextField("URL", openUrlSetting.openUrl);
                }
                else if (setting is VideoclipFromUrlSetting videoclipSetting)
                {
                    EditorGUILayout.BeginHorizontal();
                    videoclipSetting.name = EditorGUILayout.TextField("Name", videoclipSetting.name);

                    if (GUILayout.Button("SetSelectedObjectPath", GUILayout.Width(180), GUILayout.Height(20)))
                    {
                        videoclipSetting.name = GetCompletePath(Selection.activeGameObject);
                    }
                    EditorGUILayout.EndHorizontal();
                    videoclipSetting.videoclipUrl = EditorGUILayout.TextField("URL", videoclipSetting.videoclipUrl);
                    videoclipSetting.loop = EditorGUILayout.Toggle("Loop", videoclipSetting.loop);

                }
                else if (setting is TextureFromUrlSetting texSetting)
                {
                    EditorGUILayout.BeginHorizontal();
                    texSetting.name = EditorGUILayout.TextField("Name", texSetting.name);

                    if (GUILayout.Button("SetSelectedObjectPath", GUILayout.Width(180), GUILayout.Height(20)))
                    {
                        texSetting.name = GetCompletePath(Selection.activeGameObject);
                    }
                    EditorGUILayout.EndHorizontal();
                    texSetting.imageUrl = EditorGUILayout.TextField("URL", texSetting.imageUrl);
                }
                else if (setting is VideoClipControlsAnimationSetting videoCtrlSetting)
                {
                    EditorGUILayout.BeginHorizontal();
                    videoCtrlSetting.name = EditorGUILayout.TextField("Name", videoCtrlSetting.name);

                    if (GUILayout.Button("SetSelectedObjectPath", GUILayout.Width(180), GUILayout.Height(20)))
                    {
                        videoCtrlSetting.name = GetCompletePath(Selection.activeGameObject);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    videoCtrlSetting.videoPlayer = EditorGUILayout.TextField("Video Player", videoCtrlSetting.videoPlayer);

                    if (GUILayout.Button("SetSelectedObjectPath", GUILayout.Width(180), GUILayout.Height(20)))
                    {
                        videoCtrlSetting.videoPlayer = GetCompletePath(Selection.activeGameObject);
                    }
                    EditorGUILayout.EndHorizontal();


                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Frame Actions");

                    if (GUILayout.Button("Add", GUILayout.Width(180), GUILayout.Height(20)))
                    {
                        videoCtrlSetting.frameActions.Add(new FrameActionSetting());
                    }
                    if (GUILayout.Button("Remove", GUILayout.Width(180), GUILayout.Height(20)))
                    {
                        videoCtrlSetting.frameActions.RemoveAt(videoCtrlSetting.frameActions.Count - 1);
                    }
                    EditorGUILayout.EndHorizontal();


                    for (int i = 0; i < videoCtrlSetting.frameActions.Count; i++)
                    {
                        videoCtrlSetting.frameActions[i].videoTimeInSeconds = Double.Parse(EditorGUILayout.TextField("TimeInSeconds", "" + videoCtrlSetting.frameActions[i].videoTimeInSeconds));
                        videoCtrlSetting.frameActions[i].paramType = EditorGUILayout.TextField("Param Name", videoCtrlSetting.frameActions[i].animationControllerParamName);
                        videoCtrlSetting.frameActions[i].paramType = EditorGUILayout.TextField("Type NUMBER|BOOL|TRIGGER", videoCtrlSetting.frameActions[i].paramType);
                        videoCtrlSetting.frameActions[i].animationControllerParamValueNumber = Int32.Parse(EditorGUILayout.TextField("Param Number", "" + videoCtrlSetting.frameActions[i].animationControllerParamValueNumber));
                        videoCtrlSetting.frameActions[i].animationControllerParamValueBool = EditorGUILayout.Toggle("Param Bool", videoCtrlSetting.frameActions[i].animationControllerParamValueBool);
                    }
                }
                else if (setting is CustomPositionSetting posSetting)
                {
                    EditorGUILayout.BeginHorizontal();
                    posSetting.name = EditorGUILayout.TextField("Name", posSetting.name);

                    if (GUILayout.Button("SetSelectedObjectPath", GUILayout.Width(180), GUILayout.Height(20)))
                    {
                        posSetting.name = GetCompletePath(Selection.activeGameObject);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    posSetting.position[0] = EditorGUILayout.FloatField("X:", posSetting.position[0]);
                    posSetting.position[1] = EditorGUILayout.FloatField("Y:", posSetting.position[1]);
                    posSetting.position[2] = EditorGUILayout.FloatField("Z:", posSetting.position[2]);
                    EditorGUILayout.EndHorizontal();

                }
                else if (setting is CustomScaleSetting scaleSetting)
                {
                    EditorGUILayout.BeginHorizontal();
                    scaleSetting.name = EditorGUILayout.TextField("Name", scaleSetting.name);

                    if (GUILayout.Button("SetSelectedObjectPath", GUILayout.Width(180), GUILayout.Height(20)))
                    {
                        scaleSetting.name = GetCompletePath(Selection.activeGameObject);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    scaleSetting.scale[0] = EditorGUILayout.FloatField("X:", scaleSetting.scale[0]);
                    scaleSetting.scale[1] = EditorGUILayout.FloatField("Y:", scaleSetting.scale[1]);
                    scaleSetting.scale[2] = EditorGUILayout.FloatField("Z:", scaleSetting.scale[2]);
                    EditorGUILayout.EndHorizontal();
                }
                else if (setting is ReticleNearbyInfluenceBlendTree reticleNearby)
                {
                    EditorGUILayout.BeginHorizontal();
                    reticleNearby.name = EditorGUILayout.TextField("Name", reticleNearby.name);

                    if (GUILayout.Button("SetSelectedObjectPath", GUILayout.Width(180), GUILayout.Height(20)))
                    {
                        reticleNearby.name = GetCompletePath(Selection.activeGameObject);
                    }
                    EditorGUILayout.EndHorizontal();

                    reticleNearby.reticleNearByBlendTree.paramName = EditorGUILayout.TextField("ParamName", reticleNearby.reticleNearByBlendTree.paramName);
                    reticleNearby.reticleNearByBlendTree.screenWidthRatioMax = EditorGUILayout.FloatField("screenWidthRatioMax:", reticleNearby.reticleNearByBlendTree.screenWidthRatioMax);
                    reticleNearby.reticleNearByBlendTree.screenWidthRatioMin = EditorGUILayout.FloatField("screenWidthRatioMin:", reticleNearby.reticleNearByBlendTree.screenWidthRatioMin);
                    reticleNearby.reticleNearByBlendTree.zDistance = EditorGUILayout.IntField("zDistance:", reticleNearby.reticleNearByBlendTree.zDistance);
                }
                else if (setting is ReticleClickBehaviour reticleClick)
                {
                    EditorGUILayout.BeginHorizontal();
                    reticleClick.name = EditorGUILayout.TextField("Name", reticleClick.name);

                    if (GUILayout.Button("SetSelectedObjectPath", GUILayout.Width(180), GUILayout.Height(20)))
                    {
                        reticleClick.name = GetCompletePath(Selection.activeGameObject);
                    }
                    EditorGUILayout.EndHorizontal();

                    reticleClick.reticleClick.paramName = EditorGUILayout.TextField("ParamName(opt)", reticleClick.reticleClick.paramName);
                    reticleClick.reticleClick.openUrl = EditorGUILayout.TextField("Open URL(opt)", reticleClick.reticleClick.openUrl);
                }
                else if (setting is LoadRandomImageOnTapSetting loadRandomImageOnTapSetting)
                {
                    loadRandomImageOnTapSetting.name = EditorGUILayout.TextField("Name", loadRandomImageOnTapSetting.name);
                    loadRandomImageOnTapSetting.loadRandomImageOnTap = EditorGUILayout.TextField("Load Random Image On Tap", loadRandomImageOnTapSetting.loadRandomImageOnTap);
                }

                if (GUILayout.Button("Delete", GUILayout.Width(100), GUILayout.Height(20)))
                {
                    settings.Remove(setting);
                    break;
                }
                GuiLine();
            }

            customBehavioursLoadJson = EditorGUILayout.TextArea(customBehavioursLoadJson, GUILayout.Height(150));

            GuiLine();

            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.gray;
            if (GUILayout.Button("General Settings", GUILayout.Width(150), GUILayout.Height(40)))
            {
                GeneralSettings setting = new GeneralSettings();
                settings.Add(setting);
            }
            if (GUILayout.Button("Custom Thumbnail", GUILayout.Width(150), GUILayout.Height(40)))
            {
                CustomThumbnailSetting setting = new CustomThumbnailSetting();
                settings.Add(setting);
            }
            if (GUILayout.Button("Set content on click", GUILayout.Width(150), GUILayout.Height(40)))
            {
                SetContentOnClickSetting setting = new SetContentOnClickSetting();
                settings.Add(setting);
            }
            if (GUILayout.Button("Hide/Show on click", GUILayout.Width(150), GUILayout.Height(40)))
            {
                HideShowOnClickSetting setting = new HideShowOnClickSetting();
                settings.Add(setting);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.gray;

            if (GUILayout.Button("Open URL", GUILayout.Width(150), GUILayout.Height(40)))
            {
                OpenUrlSetting setting = new OpenUrlSetting();
                settings.Add(setting);
            }
            if (GUILayout.Button("Videoclip", GUILayout.Width(150), GUILayout.Height(40)))
            {
                VideoclipFromUrlSetting setting = new VideoclipFromUrlSetting();
                settings.Add(setting);
            }
            if (GUILayout.Button("Texture", GUILayout.Width(150), GUILayout.Height(40)))
            {
                TextureFromUrlSetting setting = new TextureFromUrlSetting();
                settings.Add(setting);
            }

            if (GUILayout.Button("VideoclipAnimationCtrl", GUILayout.Width(150), GUILayout.Height(40)))
            {
                VideoClipControlsAnimationSetting setting = new VideoClipControlsAnimationSetting();
                settings.Add(setting);
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.gray;

            if (GUILayout.Button("Position", GUILayout.Width(150), GUILayout.Height(40)))
            {
                CustomPositionSetting setting = new CustomPositionSetting();
                settings.Add(setting);
            }
            if (GUILayout.Button("Scale", GUILayout.Width(150), GUILayout.Height(40)))
            {
                CustomScaleSetting setting = new CustomScaleSetting();
                settings.Add(setting);
            }
            if (GUILayout.Button("Reticle Near By", GUILayout.Width(150), GUILayout.Height(40)))
            {
                ReticleNearbyInfluenceBlendTree setting = new ReticleNearbyInfluenceBlendTree();
                settings.Add(setting);
            }
            if (GUILayout.Button("Reticle Click", GUILayout.Width(150), GUILayout.Height(40)))
            {
                ReticleClickBehaviour setting = new ReticleClickBehaviour();
                settings.Add(setting);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.gray;
            if (GUILayout.Button("Load Random Image On Tap", GUILayout.Width(302), GUILayout.Height(40)))
            {
                LoadRandomImageOnTapSetting setting = new LoadRandomImageOnTapSetting();
                settings.Add(setting);
            }

            EditorGUILayout.EndHorizontal();

            GuiLine();

            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.blue;
            if (GUILayout.Button("Load JSON", GUILayout.Width(150), GUILayout.Height(40)))
            {
                Deserialize(customBehavioursLoadJson);
            }
            if (GUILayout.Button("Print", GUILayout.Width(150), GUILayout.Height(40)))
            {
                Debug.Log(Serialize(settings));
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.EndScrollView();
        }

        //Most elements don't have the main parent
        private string GetCompletePath(GameObject activeGameObject, int startDepth = 1)
        {
            List<string> names = new List<string>();

            Transform current = activeGameObject.transform;
            while (current != null)
            {
                names.Add(current.gameObject.name);
                current = current.parent;
            }

            string path = "";
            for (int i = names.Count - 1 - startDepth; i >= 0; i--)
            {
                path += names[i];
                if (i != 0)
                {
                    path += "/";
                }
            }

            return path;
        }

        private string Serialize(List<ISetting> settings)
        {
            string json = "[\n";

            for (int i = 0; i < settings.Count; i++)
            {
                ISetting setting = settings[i];
                if (setting is GeneralSettings)
                {
                    json += JsonUtility.ToJson((GeneralSettings)setting);
                }
                else if (setting is CustomThumbnailSetting)
                {
                    json += JsonUtility.ToJson((CustomThumbnailSetting)setting);
                }
                else if (setting is SetContentOnClickSetting)
                {
                    json += JsonUtility.ToJson((SetContentOnClickSetting)setting);
                }
                else if (setting is HideShowOnClickSetting)
                {
                    json += JsonUtility.ToJson((HideShowOnClickSetting)setting);
                }
                else if (setting is OpenUrlSetting)
                {
                    json += JsonUtility.ToJson((OpenUrlSetting)setting);
                }
                else if (setting is VideoclipFromUrlSetting)
                {
                    json += JsonUtility.ToJson((VideoclipFromUrlSetting)setting);
                }
                else if (setting is TextureFromUrlSetting)
                {
                    json += JsonUtility.ToJson((TextureFromUrlSetting)setting);
                }
                else if (setting is VideoClipControlsAnimationSetting)
                {
                    json += JsonUtility.ToJson((VideoClipControlsAnimationSetting)setting, true);
                }
                else if (setting is CustomPositionSetting)
                {
                    json += JsonUtility.ToJson((CustomPositionSetting)setting, true);
                }
                else if (setting is CustomScaleSetting)
                {
                    json += JsonUtility.ToJson((CustomScaleSetting)setting, true);
                }
                else if (setting is ReticleNearbyInfluenceBlendTree)
                {
                    json += JsonUtility.ToJson((ReticleNearbyInfluenceBlendTree)setting, true);
                }
                else if (setting is ReticleClickBehaviour)
                {
                    json += JsonUtility.ToJson((ReticleClickBehaviour)setting, true);
                }
                else if (setting is LoadRandomImageOnTapSetting)
                {
                    json += JsonUtility.ToJson((LoadRandomImageOnTapSetting)setting, true);
                }

                if (i < settings.Count - 1)
                {
                    json += ",";
                }
                json += "\n";
            }

            json += "]";
            return json;
        }


        private void Deserialize(string customBehavioursLoadJson)
        {
            settings = JsonUtility.FromJson<List<ISetting>>(customBehavioursLoadJson);
        }

        private void GuiLine(int i_height = 1)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, i_height);
            rect.height = i_height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }
    }
}