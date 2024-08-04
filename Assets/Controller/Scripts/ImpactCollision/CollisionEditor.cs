using System.Collections.Generic;
using Controller.Scripts.ImpactCollision.Services;
using Controller.Scripts.Utils;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Controller.Scripts.ImpactCollision
{
    [CustomEditor(typeof(CollisionManager))]
    [CanEditMultipleObjects]
    public class CollisionEditor : TankComponentEditor
    {
        // Collision Manager variables
        private SerializedProperty _armorSections;
        private SerializedProperty _colliderColor;
        private SerializedProperty _defaultArmorSection;
        private SerializedProperty _vertices;
        private SerializedProperty _useColliderVertices;

        // Managers
        private CollisionManager _collisionManager;

        // Temporary Editor variables
        private readonly List<int> _highlightedSections = new();
        private readonly List<Vector3> _selectedVertices = new();

        private bool _showVertexSettings;
        private bool _showArmorSettings;

        private bool _isAddingVertex;
        private bool _addedVertex;
        private Vector3 _newVertex;

        private void OnEnable()
        {
            _armorSections = serializedObject.FindProperty("armorSections");
            _defaultArmorSection = serializedObject.FindProperty("defaultArmorSection");

            _colliderColor = serializedObject.FindProperty("colliderColor");

            _vertices = serializedObject.FindProperty("vertices");
            _useColliderVertices = serializedObject.FindProperty("useColliderVertices");

            _collisionManager = (CollisionManager)target;
            transform = ((CollisionManager)target).gameObject.transform;
        }

        private void OnDisable()
        {
            _highlightedSections.Clear();
            _selectedVertices.Clear();
            _isAddingVertex = false;
            _addedVertex = false;
            _showVertexSettings = false;
            _showArmorSettings = false;
        }

        public void OnSceneGUI()
        {
            // Makes sure visual helpers are skipped outside prefab mode.
            if (IsInPrefabStage())
                return;

            CreateVertex();
            ShowVertices();
            ShowArmorSections();
        }

        public override void SetUpGUI()
        {
            _showVertexSettings = EditorGUILayout.Foldout(
                _showVertexSettings,
                CollisionMessages.VertexSettings
            );
            if (_showVertexSettings)
                VertexGUI();

            _showArmorSettings = EditorGUILayout.Foldout(
                _showArmorSettings,
                CollisionMessages.ArmorSettings
            );
            if (_showArmorSettings)
                ArmorGUI();
        }

        private void VertexGUI()
        {
            GUIUtils.PropFieldGUI(_colliderColor, CollisionMessages.ColliderColor);
            GUIUtils.PropFieldGUI(_vertices, CollisionMessages.LocalVertices);

            if (GUILayout.Button(CollisionMessages.UseColliderVertices))
                AddColliderVertices();

            if (CollisionGUI.AddVertexButton(_isAddingVertex))
                _isAddingVertex = !_isAddingVertex;

            if (GUILayout.Button(CollisionMessages.ClearVertices))
                _vertices.ClearArray();
        }

        private void ArmorGUI()
        {
            GUIUtils.HeaderGUI(CollisionMessages.DefaultArmorSection);
            ArmorSectionGUI(_defaultArmorSection, 0, true);

            GUIUtils.HeaderGUI(CollisionMessages.ArmorSections);
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
            GUIUtils.UpdateAllGUI();
        }

        private void ArmorSectionGUI(
            SerializedProperty armorSection,
            int index,
            bool isDefault = false
        )
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Armor Section " + (index + 1), EditorStyles.boldLabel);

            GUIUtils.PropFieldGUI(
                armorSection.FindPropertyRelative("thickness"),
                CollisionMessages.Thickness
            );
            GUIUtils.PropFieldGUI(
                armorSection.FindPropertyRelative("tolerance"),
                CollisionMessages.Tolerance
            );
            GUIUtils.PropFieldGUI(
                armorSection.FindPropertyRelative("armorMaterialType"),
                CollisionMessages.MaterialType
            );

            if (!isDefault)
            {
                var connectingPoints = armorSection.FindPropertyRelative("connectingPoints");
                GUIUtils.PropFieldGUI(connectingPoints, CollisionMessages.ConnectingPoints);
            }

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

        private void CreateVertex()
        {
            if (!_isAddingVertex)
                return;

            var guiEvent = Event.current;
            var worldRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(worldRay, out hitInfo))
            {
                var worldPoint = hitInfo.point;

                if (
                    Handles.Button(
                        worldPoint,
                        Quaternion.identity,
                        0.1f,
                        0.1f,
                        Handles.SphereHandleCap
                    )
                )
                {
                    _newVertex = worldPoint;
                    _isAddingVertex = false;
                    _addedVertex = true;
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

                if (isSelected)
                    HandleVertexSelection(vertexPoint, Color.red, true);
                else
                    HandleVertexSelection(vertexPoint, Color.green, false);
            }

            if (_selectedVertices.Count >= 4)
            {
                var newSection = new ArmorSection(_selectedVertices.GetRange(0, 4), 0);
                _collisionManager.armorSections.Add(newSection);

                _selectedVertices.Clear();

                EditorSceneManager.MarkSceneDirty(_collisionManager.gameObject.scene);
            }
        }

        private void HandleVertexSelection(Vector3 localPoint, Color handleColor, bool isSelected)
        {
            Handles.color = handleColor;
            var worldPoint = transform.TransformPoint(localPoint);
            if (
                Handles.Button(worldPoint, Quaternion.identity, 0.1f, 0.1f, Handles.SphereHandleCap)
            )
            {
                if (isSelected)
                    _selectedVertices.Remove(localPoint);
                else
                    _selectedVertices.Add(localPoint);
            }
        }

        private void ShowArmorSections()
        {
            var index = 0;
            // foreach (SerializedProperty armorSection in _armorSections)
            // {
            //     if (armorSection == null)
            //         continue;
            //
            //     if (_highlightedSections.Contains(index))
            //     {
            //         index++;
            //         continue;
            //     }
            //
            //     var connectingPoints = armorSection.FindPropertyRelative("connectingPoints");
            //
            //     Handles.color = _colliderColor.colorValue;
            //
            //     DrawConnectingPoints(connectingPoints, armorSection);
            //
            //     index++;
            // }

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

                Color fillColor = _highlightedSections.Contains(index) ? Color.yellow : Color.green;
                Color outlineColor = _colliderColor.colorValue;

                DrawSolidPlate(vertices, fillColor, outlineColor);

                index++;
            }

            foreach (var highlightedIndex in _highlightedSections)
            {
                var highlightedArmorSection = _armorSections.GetArrayElementAtIndex(
                    highlightedIndex
                );

                if (highlightedArmorSection == null)
                    continue;

                var connectingPoints = highlightedArmorSection.FindPropertyRelative(
                    "connectingPoints"
                );

                Handles.color = Color.yellow;

                DrawConnectingPoints(connectingPoints, highlightedArmorSection);
            }
        }

        private void DrawSolidPlate(Vector3[] vertices, Color fillColor, Color outlineColor)
        {
            if (vertices.Length < 4)
                return;

            // Convert vertices to world coordinates
            Vector3[] worldVertices = new Vector3[4];
            for (int i = 0; i < 4; i++)
            {
                worldVertices[i] = transform.TransformPoint(vertices[i]);
            }

            // Set the alpha for semi-transparency
            fillColor.a = 0.1f; // 50% transparent
            outlineColor.a = 1.0f; // Fully opaque outline

            // Draw the solid rectangle with outline
            Handles.DrawSolidRectangleWithOutline(worldVertices, fillColor, outlineColor);
        }

        private void DrawConnectingPoints(
            SerializedProperty connectingPoints,
            SerializedProperty armorSection
        )
        {
            for (var i = 0; i < connectingPoints.arraySize; i++)
            {
                if (i == connectingPoints.arraySize - 1)
                    continue;

                var connectingPoint = connectingPoints.GetArrayElementAtIndex(i).vector3Value;
                var nextConnectingPoint = connectingPoints
                    .GetArrayElementAtIndex(1 + i)
                    .vector3Value;

                Handles.DrawLine(
                    connectingPoint + transform.position,
                    nextConnectingPoint + transform.position,
                    3f
                );
            }

            Handles.DrawLine(
                connectingPoints.GetArrayElementAtIndex(0).vector3Value + transform.position,
                connectingPoints.GetArrayElementAtIndex(connectingPoints.arraySize - 1).vector3Value
                    + transform.position,
                3f
            );
        }

        public override void ApplyUpdate()
        {
            if (_addedVertex)
            {
                var index = _vertices.arraySize;
                _vertices.InsertArrayElementAtIndex(index);
                _vertices.GetArrayElementAtIndex(index).vector3Value =
                    transform.InverseTransformPoint(_newVertex);
                _addedVertex = false;
            }
        }

        public void AddColliderVertices()
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
