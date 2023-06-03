using Controller.Scripts.Editors.Wheels;
using UnityEditor;
using UnityEngine;

namespace Controller.Scripts.Editors.Utils
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
    
        public static class DrawUtils
    {
        public static void ShowLabel(GameObject gameObject, SerializedProperty showLabel)
        {
            if (!showLabel.boolValue) return;
            
            var iconContent = EditorGUIUtility.IconContent(WheelUtilsMessages.ShowLabelType);
            EditorGUIUtility.SetIconForObject(gameObject, (Texture2D) iconContent.image);
        }
        
        public static void DrawArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius, Color color)
        {
            Handles.color = color;
            Handles.DrawSolidArc(center, normal, from, angle, radius);
        }
        
        public static void DrawLine(Vector3 start, Vector3 end, Color color, int width = 1)
        {
            Handles.color = color;
            Handles.DrawLine(start, end, width);
        }
        
        public static void DrawCircleWithDirection(Transform transform, Vector3 circleNormal, Vector3 torqueDir, float radius)
        {
            Vector3 worldNormal = transform.TransformDirection(circleNormal);
            
            if(circleNormal != Vector3.zero)
                DrawCircle(transform, worldNormal, radius, GeneralMessages.LightRed);
                
            if(torqueDir != Vector3.zero)
                DrawArrow(transform.position, torqueDir, 0.1f, 0.01f);
        }

        public static void DrawCircle(Transform transform, Vector3 normal, float radius, Color color)
        {
            Handles.color = color;
            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, normal);
            Handles.DrawSolidArc(transform.position, normal, rotation * Vector3.right, 360, radius);        
        }
        
        public static void DrawArrow(Vector3 startPosition, Vector3 direction, float distance, float arrowheadSize)
        {
            Vector3 endPosition = startPosition + direction.normalized * distance;

            Handles.color = Color.blue;
            Handles.DrawLine(startPosition, endPosition);

            DrawArrowhead(endPosition, direction, arrowheadSize);
        }

        private static void DrawArrowhead(Vector3 position, Vector3 direction, float size)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);

            Vector3 right = rotation * Quaternion.Euler(0, 145, 0) * Vector3.forward;
            Vector3 left = rotation * Quaternion.Euler(0, -145, 0) * Vector3.forward;

            Handles.DrawLine(position, position + right * size);
            Handles.DrawLine(position, position + left * size);
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
        
        public static readonly Color LightRed = new Color(1f, 0.5f, 0.5f, 0.3f);
        public static readonly Color Orange = new Color(1f, 0.3f, 0f, 0.3f);
    }
}