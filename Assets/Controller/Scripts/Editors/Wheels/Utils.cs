using UnityEditor;
using UnityEngine;

namespace Controller.Scripts.Editors.Wheels
{
    
    public enum WheelType
    {
        SupportWheel,
        DriveWheel,
        SuspensionWheel,
        RearWheel,
    }
    
    public static class WheelUtilsMessages 
    {
        public const string GeneralSettings = "General Settings";
        public const string GeneralWheelSettings = "General Wheel Settings";
        public const string RightWheelSettings = "Right Wheel Settings";
        public const string LeftWheelSettings = "Left Wheel Settings";
        public const string GeneralSuspensionSettings = "General Suspension Settings";
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
        public const string HingeAxis = "Hinge Axis";

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
    
    public static class Utils
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
                DrawCircle(transform, worldNormal, radius, WheelUtilsMessages.LightRed);
                
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
}