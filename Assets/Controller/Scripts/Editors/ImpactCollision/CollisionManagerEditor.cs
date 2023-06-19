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

        private List<Vector3> _selectedVertices = new ();

        private void OnEnable()
        {
            _armorSections = serializedObject.FindProperty("armorSections");
            _colliderColor = serializedObject.FindProperty("colliderColor");
            _defaultArmorSection = serializedObject.FindProperty("defaultArmorSection");

            transform = ((CollisionManager)target).gameObject.transform;
        }

        public override void SetUpGUI()
        {
            GUIUtils.PropFieldGUI(_colliderColor, "Collider Color");
            
            GUIUtils.HeaderGUI("Default Armor Section");
            ArmorSectionGUI(_defaultArmorSection, 0, true);
            
            GUIUtils.HeaderGUI("Armor Sections");
            for (int i = 0; i < _armorSections.arraySize; i++)
            {
                SerializedProperty armorSection = _armorSections.GetArrayElementAtIndex(i);
                if (armorSection == null)
                    continue;
                ArmorSectionGUI(armorSection, i);
                RemoveArmorSection(i);
            }
        }

        private void ArmorSectionGUI(SerializedProperty armorSection, int index, bool isDefault = false)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Armor Section " + (index + 1), EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(armorSection.FindPropertyRelative("thickness"), new GUIContent("Thickness"));
            EditorGUILayout.PropertyField(armorSection.FindPropertyRelative("useColliderAngle"), new GUIContent("Use Collider Angle"));
            if (!armorSection.FindPropertyRelative("useColliderAngle").boolValue)
            {
                EditorGUILayout.PropertyField(armorSection.FindPropertyRelative("angle"), new GUIContent("Collider Angle"));
                EditorGUILayout.PropertyField(armorSection.FindPropertyRelative("rotationAxis"), new GUIContent("Rotation Axis"));
            }

            EditorGUILayout.PropertyField(armorSection.FindPropertyRelative("tolerance"), new GUIContent("Tolerance"));
            EditorGUILayout.PropertyField(armorSection.FindPropertyRelative("armorMaterialType"), new GUIContent("Material Type"));
            

            if (!isDefault)
            {
                SerializedProperty connectingPoints = armorSection.FindPropertyRelative("connectingPoints");
                EditorGUILayout.PropertyField(connectingPoints, new GUIContent("Connecting Points"), true);
            }
            
            EditorGUILayout.EndVertical();
        }

        private void RemoveArmorSection(int i)
        {
            if (GUILayout.Button("Remove"))
            {
                _armorSections.DeleteArrayElementAtIndex(i);
            }
        }

        public void OnSceneGUI()
        {
            CollisionManager collisionManager = (CollisionManager)target;
            MeshCollider[] meshColliders = collisionManager.GetComponentsInChildren<MeshCollider>();

            if (meshColliders.Length == 0) return;

            foreach (MeshCollider meshCollider in meshColliders)
            {
                if (meshCollider.sharedMesh == null) continue;

                foreach (Vector3 vertex in meshCollider.sharedMesh.vertices)
                {
                    Vector3 worldPoint = meshCollider.transform.TransformPoint(vertex);
                    Vector3 localPoint = vertex;
                    
                    bool isSelected = _selectedVertices.Contains(localPoint);

                    if (isSelected)
                    {
                        Handles.color = Color.red;
                        if (Handles.Button(worldPoint, Quaternion.identity, 0.1f, 0.1f, Handles.SphereHandleCap))
                        {
                            _selectedVertices.Remove(localPoint);
                        }                  
                    }
                    else
                    {
                        Handles.color = Color.green;
                        if (Handles.Button(worldPoint, Quaternion.identity, 0.1f, 0.1f, Handles.SphereHandleCap))
                        {
                            _selectedVertices.Add(localPoint);
                        }
                    }
                }
            }

            if (_selectedVertices.Count >= 4)
            {
                ArmorSection newSection = new ArmorSection(_selectedVertices.GetRange(0, 4), 0, 0);
                collisionManager.armorSections.Add(newSection);
                
                _selectedVertices.Clear();

                EditorSceneManager.MarkSceneDirty(collisionManager.gameObject.scene);
            }

            ShowArmorSections();
        }


        private void ShowArmorSections()
        {
            foreach(SerializedProperty armorSection in _armorSections)
            {
                if (armorSection == null)
                    continue;

                SerializedProperty connectingPoints = armorSection.FindPropertyRelative("connectingPoints");
                
                Handles.color = _colliderColor.colorValue;

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
        }
    }
}
