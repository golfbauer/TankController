using UnityEditor;
using UnityEngine;

namespace Controller.Scripts.Editors.Wheels
{
    public static class WheelUtilsMessages 
    {
        public const string Settings = "Settings";
        public const string GeneralSettings = "General Settings";
        public const string GeneralWheelSettings = "General Wheel Settings";
        public const string RightWheelSettings = "Right Wheel Settings";
        public const string LeftWheelSettings = "Left Wheel Settings";
        public const string SuspensionSettings = "Suspension Settings";
        public const string RightSuspensionSettings = "Right Suspension Settings";
        public const string LeftSuspensionSettings = "Left Suspension Settings";
        
        public const string Spacing = "Spacing";
        public const string Distance = "Distance";
        public const string Offset = "Offset";
        public const string Count = "Count";
        
        public const string ColliderRadius = "Collider Radius";
        public const string ColliderMaterial = "Collider Material";
        
        public const string SpringTargetPosition = "Spring Target Position";
        public const string SpringMaxAngle = "Spring Max Angle";
        public const string SpringMinAngle = "Spring Min Angle";
        
        public const string EulerRotation = "Euler Rotation";
        public const string TorqueDirection = "Torque Direction";

        public const string Mass = "Mass";

        public const string Mesh = "Mesh";
        public const string Material = "Material";

        public const string NotPrefabModeWarning = "You must be in prefab mode to use this tool.";
        public const string UpdateAll = "Update All";
        public const string ShowLabelType = "sv_label_1";
        
        public const string LeftWheel = "L Wheel ";
        public const string RightWheel = "R Wheel ";
        public const string LeftSuspension = "L Suspension ";
        public const string RightSuspension = "R Suspension ";
        
        public static readonly Color LightRed = new Color(1f, 0.5f, 0.5f, 0.3f);
        public static readonly Color Green = Color.green;
        public static readonly Color Orange = new Color(1f, 0.3f, 0f, 0.3f);
    }
    
    public static class WheelsUtils
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
    }
}