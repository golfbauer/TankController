using System.Collections.Generic;
using Controller.Scripts.Editors.Utils;
using Controller.Scripts.Managers.ImpactCollision;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Controller.Scripts.Editors.ImpactCollision
{
    [CustomEditor(typeof(CollisionManager))]
    [CanEditMultipleObjects]
    public class CollisionManagerEditor : TankComponentEditor
    {
        private SerializedProperty _armorSections;
        private SerializedProperty _colliderColor;
        private SerializedProperty _defaultArmorSection;
        private SerializedProperty _vertices;
        private SerializedProperty _useColliderVertices;

        private List<Vector3> _selectedVertices = new ();
        private List<int> _highlightedSections = new ();
        
        private bool _addVertex;
        private Vector3 _newVertex;
        
        private CollisionManager _collisionManager;

        private void OnEnable()
        {
            _armorSections = serializedObject.FindProperty("armorSections");
            _colliderColor = serializedObject.FindProperty("colliderColor");
            _defaultArmorSection = serializedObject.FindProperty("defaultArmorSection");
            _vertices = serializedObject.FindProperty("vertices");
            _useColliderVertices = serializedObject.FindProperty("useColliderVertices");

            transform = ((CollisionManager)target).gameObject.transform;
            _collisionManager = (CollisionManager) target;
        }

        public override void SetUpGUI()
        {
            GUIUtils.HeaderGUI(CollisionMessages.VertexSettings);
            VertexGUI();
            
            GUIUtils.HeaderGUI(CollisionMessages.DefaultArmorSection);
            ArmorSectionGUI(_defaultArmorSection, 0, true);
            
            GUIUtils.HeaderGUI(CollisionMessages.ArmorSections);
            for (int i = 0; i < _armorSections.arraySize; i++)
            {
                SerializedProperty armorSection = _armorSections.GetArrayElementAtIndex(i);
                if (armorSection == null)
                    continue;
                ArmorSectionGUI(armorSection, i);
                EditorGUILayout.BeginHorizontal();
                Highlight(i);
                Remove(i);
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.Space();
            if(GUILayout.Button("Clear Armor Sections"))
                _armorSections.ClearArray();
            GUIUtils.UpdateAllGUI();
        }

        private void VertexGUI()
        {
            GUIUtils.PropFieldGUI(_colliderColor, CollisionMessages.ColliderColor);
            GUIUtils.PropFieldGUI(_vertices);
            GUIUtils.PropFieldGUI(_useColliderVertices, CollisionMessages.UseColliderVertices);
            
            if(GUILayout.Button(!_addVertex ? CollisionMessages.AddVertex : GeneralMessages.Cancel))
                _addVertex = true;
            
            if(GUILayout.Button(CollisionMessages.ClearVertices))
                _vertices.ClearArray();
        }

        private void ArmorSectionGUI(SerializedProperty armorSection, int index, bool isDefault = false)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Armor Section " + (index + 1), EditorStyles.boldLabel);

            GUIUtils.PropFieldGUI(armorSection.FindPropertyRelative("thickness"), CollisionMessages.Thickness);
            GUIUtils.PropFieldGUI(armorSection.FindPropertyRelative("useColliderAngle"), CollisionMessages.UseColliderAngle);
            if (!armorSection.FindPropertyRelative("useColliderAngle").boolValue)
            {
                GUIUtils.PropFieldGUI(armorSection.FindPropertyRelative("angle"), CollisionMessages.ColliderAngle);
                GUIUtils.PropFieldGUI(armorSection.FindPropertyRelative("rotationAxis"), CollisionMessages.RotationAxis);
            }
            
            GUIUtils.PropFieldGUI(armorSection.FindPropertyRelative("tolerance"), CollisionMessages.Tolerance);
            GUIUtils.PropFieldGUI(armorSection.FindPropertyRelative("armorMaterialType"), CollisionMessages.MaterialType);

            if (!isDefault)
            {
                SerializedProperty connectingPoints = armorSection.FindPropertyRelative("connectingPoints");
                GUIUtils.PropFieldGUI(connectingPoints, CollisionMessages.ConnectingPoints);
            }
            
            EditorGUILayout.EndVertical();
        }

        private void Remove(int i)
        {
            if (GUILayout.Button(GeneralMessages.Remove))
            {
                _armorSections.DeleteArrayElementAtIndex(i);
            }
        }

        private void Highlight(int index)
        {
            if (GUILayout.Button(_highlightedSections.Contains(index) ? CollisionMessages.UnHighlight : CollisionMessages.Highlight))
            {
                if (_highlightedSections.Contains(index))
                    _highlightedSections.Remove(index);
                else
                    _highlightedSections.Add(index);
            }
        }

        public void OnSceneGUI()
        {
            if (AllowAccess())
                return;
            
            CreateVertex();
            ShowVertices();
            ShowArmorSections();
        }
        
        private void CreateVertex()
        {
            if (!_addVertex)
                return;
            
            Event guiEvent = Event.current;
            Ray worldRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
            RaycastHit hitInfo;
    
            if (Physics.Raycast(worldRay, out hitInfo))
            {
                Vector3 worldPoint = hitInfo.point;

                if (Handles.Button(worldPoint, Quaternion.identity, 0.1f, 0.1f, Handles.SphereHandleCap))
                {
                    _newVertex = worldPoint;
                    _addVertex = false;
                    updateAll = true;
                    Repaint();
                }
            }
        }

        private void ShowVertices()
        {
            for(int i = 0; i < _vertices.arraySize; i++)
            {
                Vector3 worldPoint = _vertices.GetArrayElementAtIndex(i).vector3Value;
                Vector3 localPoint = transform.InverseTransformPoint(worldPoint);
                
                bool isSelected = _selectedVertices.Contains(localPoint);

                if (isSelected)
                    HandleVertexSelection(localPoint, worldPoint, Color.red, true);
                else
                    HandleVertexSelection(localPoint, worldPoint, Color.green, false);
            }

            if (_selectedVertices.Count >= 4)
            {
                ArmorSection newSection = new ArmorSection(_selectedVertices.GetRange(0, 4), 0, 0);
                _collisionManager.armorSections.Add(newSection);
                
                _selectedVertices.Clear();

                EditorSceneManager.MarkSceneDirty(_collisionManager.gameObject.scene);
            }
        } 
        
        private void HandleVertexSelection(Vector3 localPoint, Vector3 worldPoint, Color handleColor, bool isSelected)
        {
            Handles.color = handleColor;
            if (Handles.Button(worldPoint, Quaternion.identity, 0.1f, 0.1f, Handles.SphereHandleCap))
            {
                if(isSelected) 
                    _selectedVertices.Remove(localPoint);
                else
                    _selectedVertices.Add(localPoint);
            }
        }
        
        private void ShowArmorSections()
        {
            int index = 0;
            foreach(SerializedProperty armorSection in _armorSections)
            {
                if (armorSection == null)
                    continue;

                if (_highlightedSections.Contains(index))
                {
                    index++;
                    continue;
                }

                SerializedProperty connectingPoints = armorSection.FindPropertyRelative("connectingPoints");
                
                Handles.color = _colliderColor.colorValue;

                DrawConnectingPoints(connectingPoints, armorSection);

                index++;
            }
            
            foreach(int highlightedIndex in _highlightedSections)
            {
                SerializedProperty highlightedArmorSection = _armorSections.GetArrayElementAtIndex(highlightedIndex);

                if (highlightedArmorSection == null)
                    continue;

                SerializedProperty connectingPoints = highlightedArmorSection.FindPropertyRelative("connectingPoints");

                Handles.color = Color.yellow;
                
                DrawConnectingPoints(connectingPoints, highlightedArmorSection);
            }
        }

        private void DrawConnectingPoints(SerializedProperty connectingPoints, SerializedProperty armorSection)
        {
            for (int i = 0; i < connectingPoints.arraySize; i++)
            {
                if(i == connectingPoints.arraySize - 1)
                    continue;
                
                Vector3 connectingPoint = connectingPoints.GetArrayElementAtIndex(i).vector3Value;
                Vector3 nextConnectingPoint = connectingPoints.GetArrayElementAtIndex(1 + i).vector3Value;
                
                Handles.DrawLine(connectingPoint + transform.position, nextConnectingPoint + transform.position, 3f);
            }

            Handles.DrawLine(
                connectingPoints.GetArrayElementAtIndex(0).vector3Value + transform.position, 
                connectingPoints.GetArrayElementAtIndex(connectingPoints.arraySize - 1).vector3Value + transform.position,
                3f
            );

            if (armorSection.FindPropertyRelative("useColliderAngle").boolValue)
            {
                Handles.DrawLine(
                    connectingPoints.GetArrayElementAtIndex(0).vector3Value, 
                    connectingPoints.GetArrayElementAtIndex(0).vector3Value +
                    armorSection.FindPropertyRelative("rotationAxis").vector3Value * 2f
                );
            }
        }

        private void OnDisable()
        {
            _highlightedSections.Clear();
        }

        public override void BulkUpdateComponents()
        {
            if (_newVertex != Vector3.zero)
            {
                int index = _vertices.arraySize;
                _vertices.InsertArrayElementAtIndex(index);
                _vertices.GetArrayElementAtIndex(index).vector3Value = _newVertex;
                _newVertex = Vector3.zero;
            }

            if(!_useColliderVertices.boolValue)
                return;
            
            MeshCollider[] meshColliders = transform.GetComponentsInChildren<MeshCollider>();

            if (meshColliders.Length == 0) return;

            foreach (MeshCollider meshCollider in meshColliders)
            {
                if (meshCollider.sharedMesh == null) continue;

                foreach (Vector3 vertex in meshCollider.sharedMesh.vertices)
                {
                    Vector3 worldPoint = meshCollider.transform.TransformPoint(vertex);
                    bool exists = false;
                    for (int i = 0; i < _vertices.arraySize; i++)
                    {
                        Vector3 existingVertex = _vertices.GetArrayElementAtIndex(i).vector3Value;
                        if (Vector3.Distance(worldPoint, existingVertex) < 0.001f)
                        {
                            exists = true;
                            break;
                        }
                    }
                    
                    if (!exists)
                    {
                        int index = _vertices.arraySize;
                        _vertices.InsertArrayElementAtIndex(index);
                        _vertices.GetArrayElementAtIndex(index).vector3Value = worldPoint;
                    }
                }
            }
        }
    }
}
