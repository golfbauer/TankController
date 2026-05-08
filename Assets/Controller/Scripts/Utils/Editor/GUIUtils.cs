using UnityEditor;
using UnityEngine;

namespace Controller.Scripts.Utils
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

        public static void SliderGUI(
            SerializedProperty property,
            float leftVal,
            float rightVal,
            string label
        )
        {
            EditorGUILayout.Slider(property, leftVal, rightVal, label);
        }

        public static void IntSliderGUI(
            SerializedProperty property,
            int leftVal,
            int rightVal,
            string label
        )
        {
            EditorGUILayout.IntSlider(property, leftVal, rightVal, label);
        }

        public static void DenyAccessGUI(
            string forbiddenResponse = GeneralMessages.NotPrefabModeWarning
        )
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

        public static void Space(int space = 1)
        {
            EditorGUILayout.Space(space);
        }
    }

    public static class DrawUtils
    {
        public static void ShowLabel(GameObject gameObject, SerializedProperty showLabel)
        {
            if (!showLabel.boolValue)
                return;

            var iconContent = EditorGUIUtility.IconContent(GeneralMessages.ShowLabelType);
            EditorGUIUtility.SetIconForObject(gameObject, (Texture2D)iconContent.image);
        }

        public static void DrawArc(
            Vector3 center,
            Vector3 normal,
            Vector3 from,
            float angle,
            float radius,
            Color color
        )
        {
            Handles.color = color;
            Handles.DrawSolidArc(center, normal, from, angle, radius);
        }

        public static void DrawLine(Vector3 start, Vector3 end, Color color, int width = 1)
        {
            Handles.color = color;
            Handles.DrawLine(start, end, width);
        }

        public static void DrawCircleWithDirection(
            Transform transform,
            Vector3 circleNormal,
            Vector3 torqueDir,
            float radius
        )
        {
            var worldNormal = transform.TransformDirection(circleNormal);

            if (circleNormal != Vector3.zero)
                DrawCircle(transform, worldNormal, radius, GeneralMessages.LightRed);

            if (torqueDir != Vector3.zero)
                DrawArrow(transform.position, torqueDir, 0.1f, 0.01f);
        }

        public static void DrawCircle(
            Transform transform,
            Vector3 normal,
            float radius,
            Color color
        )
        {
            Handles.color = color;
            var rotation = Quaternion.FromToRotation(Vector3.forward, normal);
            Handles.DrawSolidArc(transform.position, normal, rotation * Vector3.right, 360, radius);
        }

        public static void DrawArrow(
            Vector3 startPosition,
            Vector3 direction,
            float distance,
            float arrowheadSize
        )
        {
            var endPosition = startPosition + direction.normalized * distance;

            Handles.color = Color.blue;
            Handles.DrawLine(startPosition, endPosition);

            DrawArrowhead(endPosition, direction, arrowheadSize);
        }

        private static void DrawArrowhead(Vector3 position, Vector3 direction, float size)
        {
            var rotation = Quaternion.LookRotation(direction);

            var right = rotation * Quaternion.Euler(0, 145, 0) * Vector3.forward;
            var left = rotation * Quaternion.Euler(0, -145, 0) * Vector3.forward;

            Handles.DrawLine(position, position + right * size);
            Handles.DrawLine(position, position + left * size);
        }
    }
}
