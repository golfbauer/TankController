using System.Collections.Generic;
using Controller.Scripts.ImpactCollision.Services;
using Controller.Scripts.Utils;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Controller.Scripts.ImpactCollision
{
    [CustomEditor(typeof(CollisionManager))]
    public class CollisionEditor : TankComponentEditor
    {
        // Managers
        private CollisionManager _collisionManager;

        // Collision Manager variables
        private SerializedProperty _armorSections;
        private SerializedProperty _defaultArmorSection;

        private SerializedProperty _vertices;
        private SerializedProperty _colorThicknessModifier;
        private SerializedProperty _showArmorSettingSection;

        // Editor variables
        private readonly List<int> _highlightedSections = new();
        private readonly List<Vector3> _selectedVertices = new();
        private readonly float _buttonSize = 0.07f;

        private bool _isAddingVertex;
        private Vector3 _newVertex;

        private PhysicsScene _physicsScene;

        // Editor colors
        private readonly Color _lightArmorColor = Color.red;
        private readonly Color _mediumArmorColor = Color.yellow;
        private readonly Color _heavyArmorColor = Color.green;
        private readonly Color _highlightedArmorSectionColor = Color.blue;

        private readonly Color _defaultVertexColor = Color.black;
        private readonly Color _selectedVertexColor = Color.white;

        private void OnEnable()
        {
            _armorSections = serializedObject.FindProperty("armorSections");
            _defaultArmorSection = serializedObject.FindProperty("defaultArmorSection");
            _vertices = serializedObject.FindProperty("vertices");
            _showArmorSettingSection = serializedObject.FindProperty("showArmorSettingSection");

            _colorThicknessModifier = serializedObject.FindProperty("colorThicknessModifier");

            _collisionManager = (CollisionManager)target;
            transform = ((CollisionManager)target).gameObject.transform;
            _physicsScene = transform.gameObject.scene.GetPhysicsScene();
        }

        private void OnDisable()
        {
            _highlightedSections.Clear();
            _selectedVertices.Clear();
            _isAddingVertex = false;
            _newVertex = Vector3.zero;
        }

        public void OnSceneGUI()
        {
            // Makes sure visual helpers are skipped outside prefab mode.
            if (IsInPrefabStage())
                return;

            AddNewVertex();
            ShowVertices();
            ShowArmorSections();
        }

        public override void SetUpGUI()
        {
            VertexGUI();

            EditorGUILayout.Space();

            _showArmorSettingSection.boolValue = EditorGUILayout.Foldout(
                _showArmorSettingSection.boolValue,
                CollisionMessages.ArmorSettings
            );
            if (_showArmorSettingSection.boolValue)
                ArmorGUI();

            GUIUtils.UpdateAllGUI();
        }

        private void VertexGUI()
        {
            GUIUtils.PropFieldGUI(_vertices, CollisionMessages.LocalVertices);

            if (GUILayout.Button(CollisionMessages.UseColliderVertices))
                AddColliderVertices();

            if (
                GUILayout.Button(
                    !_isAddingVertex ? CollisionMessages.AddVertex : GeneralMessages.Cancel
                )
            )
                _isAddingVertex = !_isAddingVertex;

            if (GUILayout.Button(CollisionMessages.ClearVertices))
                _vertices.ClearArray();
        }

        private void ArmorGUI()
        {
            GUIUtils.HeaderGUI(CollisionMessages.DefaultArmorSection);
            ArmorSectionGUI(_defaultArmorSection, 0);

            GUIUtils.HeaderGUI(CollisionMessages.ArmorSections);
            GUIUtils.PropFieldGUI(
                _colorThicknessModifier,
                CollisionMessages.ColorThicknessModifier
            );
            for (var i = 0; i < _armorSections.arraySize; i++)
            {
                var armorSection = _armorSections.GetArrayElementAtIndex(i);
                if (armorSection == null)
                    continue;
                ArmorSectionGUI(armorSection, i);
                EditorGUILayout.BeginHorizontal();
                Highlight(i);
                Remove(i);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            if (GUILayout.Button(CollisionMessages.ClearArmorSections))
                _armorSections.ClearArray();
        }

        private void ArmorSectionGUI(SerializedProperty armorSection, int index)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Armor Section " + (index + 1), EditorStyles.boldLabel);

            GUIUtils.PropFieldGUI(
                armorSection.FindPropertyRelative("thickness"),
                CollisionMessages.Thickness
            );
            GUIUtils.PropFieldGUI(
                armorSection.FindPropertyRelative("armorMaterialType"),
                CollisionMessages.MaterialType
            );

            EditorGUILayout.EndVertical();
        }

        private void Remove(int i)
        {
            if (GUILayout.Button(GeneralMessages.Remove))
                _armorSections.DeleteArrayElementAtIndex(i);
        }

        private void Highlight(int index)
        {
            if (
                GUILayout.Button(
                    _highlightedSections.Contains(index)
                        ? CollisionMessages.UnHighlight
                        : CollisionMessages.Highlight
                )
            )
            {
                if (_highlightedSections.Contains(index))
                    _highlightedSections.Remove(index);
                else
                    _highlightedSections.Add(index);
            }
        }

        private void AddNewVertex()
        {
            if (!_isAddingVertex)
                return;

            var worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            if (_physicsScene.Raycast(worldRay.origin, worldRay.direction, out var hitInfo))
            {
                if (hitInfo.transform != transform)
                    return;

                var worldPoint = hitInfo.point;

                if (
                    Handles.Button(
                        worldPoint,
                        Quaternion.identity,
                        _buttonSize,
                        0.1f,
                        Handles.SphereHandleCap
                    )
                )
                {
                    _newVertex = worldPoint;
                    _isAddingVertex = false;
                    update = true;
                    Repaint();
                }
            }
        }

        private void ShowVertices()
        {
            for (var i = 0; i < _vertices.arraySize; i++)
            {
                var vertexPoint = _vertices.GetArrayElementAtIndex(i).vector3Value;
                var isSelected = _selectedVertices.Contains(vertexPoint);

                ShowVertexButton(vertexPoint, isSelected);
            }

            if (_selectedVertices.Count >= 4)
            {
                var newSection = new ArmorSection(_selectedVertices.GetRange(0, 4));
                _collisionManager.armorSections.Add(newSection);

                _selectedVertices.Clear();

                EditorSceneManager.MarkSceneDirty(_collisionManager.gameObject.scene);
            }
        }

        private void ShowVertexButton(Vector3 localPoint, bool isSelected)
        {
            Handles.color = isSelected ? _selectedVertexColor : _defaultVertexColor;
            var worldPoint = transform.TransformPoint(localPoint);
            if (
                Handles.Button(
                    worldPoint,
                    Quaternion.identity,
                    0.07f,
                    0.1f,
                    Handles.SphereHandleCap
                )
            )
            {
                if (isSelected)
                    _selectedVertices.Remove(localPoint);
                else
                    _selectedVertices.Add(localPoint);
            }
            Handles.color = Color.white;
        }

        private void ShowArmorSections()
        {
            var index = 0;
            foreach (SerializedProperty armorSection in _armorSections)
            {
                if (armorSection == null)
                    continue;

                var connectingPoints = armorSection.FindPropertyRelative("connectingPoints");
                Vector3[] vertices = new Vector3[4];
                for (int i = 0; i < 4; i++)
                {
                    vertices[i] = connectingPoints.GetArrayElementAtIndex(i).vector3Value;
                }

                Color fillColor = _highlightedSections.Contains(index)
                    ? _highlightedArmorSectionColor
                    : GetModifiedColor(armorSection.FindPropertyRelative("thickness").floatValue);
                DrawSolidPlate(vertices, fillColor);

                index++;
            }
        }

        private Color GetModifiedColor(float armorThickness)
        {
            if (armorThickness * _colorThicknessModifier.floatValue > 74)
            {
                return _heavyArmorColor;
            }

            if (armorThickness * _colorThicknessModifier.floatValue > 29)
            {
                return _mediumArmorColor;
            }

            return _lightArmorColor;
        }

        private void DrawSolidPlate(Vector3[] vertices, Color fillColor)
        {
            if (vertices.Length < 4)
                return;

            // Calculate the centroid of the rectangle
            Vector3 centroid = Vector3.zero;
            for (int i = 0; i < 4; i++)
            {
                centroid += vertices[i];
            }
            centroid /= 4.0f;

            // Convert vertices to world coordinates with a slight inward offset
            Vector3[] worldVertices = new Vector3[4];
            for (int i = 0; i < 4; i++)
            {
                Vector3 offsetDirection = (centroid - vertices[i]).normalized;
                worldVertices[i] = transform.TransformPoint(vertices[i] + offsetDirection * 0.01f);
            }

            fillColor.a = 0.1f;
            Color outlineColor = fillColor;
            outlineColor.a = 1.0f;
            Handles.DrawSolidRectangleWithOutline(worldVertices, fillColor, outlineColor);
        }

        public override void ApplyUpdate()
        {
            if (_newVertex != Vector3.zero)
            {
                var index = _vertices.arraySize;
                _vertices.InsertArrayElementAtIndex(index);
                _vertices.GetArrayElementAtIndex(index).vector3Value =
                    transform.InverseTransformPoint(_newVertex);
                _newVertex = Vector3.zero;
            }
        }

        private void AddColliderVertices()
        {
            var meshColliders = transform.GetComponents<MeshCollider>();

            if (meshColliders.Length == 0)
                return;

            foreach (var meshCollider in meshColliders)
            {
                if (meshCollider.sharedMesh == null)
                    continue;

                foreach (var vertex in meshCollider.sharedMesh.vertices)
                {
                    var worldPoint = meshCollider.transform.TransformPoint(vertex);
                    var exists = false;
                    for (var i = 0; i < _vertices.arraySize; i++)
                    {
                        var existingVertex = _vertices.GetArrayElementAtIndex(i).vector3Value;
                        if (Vector3.Distance(worldPoint, existingVertex) < 0.001f)
                        {
                            exists = true;
                            break;
                        }
                    }

                    if (!exists)
                    {
                        var index = _vertices.arraySize;
                        _vertices.InsertArrayElementAtIndex(index);
                        _vertices.GetArrayElementAtIndex(index).vector3Value = vertex;
                    }
                }
            }
        }
    }
}
