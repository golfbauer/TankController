using Controller.Scripts.Editors.Wheels;
using UnityEditor;
using UnityEngine;

namespace Controller.Scripts.Editors
{
    public static class GUIUtils
    {
        public static void PropFieldGUI(SerializedProperty property, string label = null)
        {
            if (label == null)
                EditorGUILayout.PropertyField(property);
            else
                EditorGUILayout.PropertyField(property, new GUIContent(label));
        }

        public static void SliderGUI(SerializedProperty property, float leftVal, float rightVal, string label)
        {
            EditorGUILayout.Slider(property, leftVal, rightVal, label);
        }
        
        public static void IntSliderGUI(SerializedProperty property, int leftVal, int rightVal, string label)
        {
            EditorGUILayout.IntSlider(property, leftVal, rightVal, label);
        }

        public static void DenyAccessGUI(string forbiddenResponse = GeneralMessages.NotPrefabModeWarning)
        {
            EditorGUILayout.HelpBox(forbiddenResponse, MessageType.Info);
        }

        public static void HeaderGUI(string header)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(header, EditorStyles.boldLabel);
        }
        
        public static bool UpdateAllGUI()
        {
            return GUILayout.Button(GeneralMessages.UpdateAll);
        }
    }

    public static class LayerUtils
    {
        public const string WheelLayer = "TankWheel";
        public const string HullLayer = "TankHull";
        
        
        public static void SetLayer(GameObject gameObject, string layerName)
        {
            gameObject.layer = LayerMask.NameToLayer(layerName);
        }
    }

    public static class GeneralMessages
    {
        public const string UpdateAll = "Update All";
        public const string NotPrefabModeWarning = "You must be in prefab mode to use this tool.";
        public const string PrefabModeWarning = "You must exit prefab mode to use this tool.";
    }
}