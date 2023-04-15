using System;
using System.Collections.Generic;
using Controller.Scripts.Managers.Wheels;
using UnityEditor;
using UnityEngine;

namespace Controller.Scripts.Editors.Wheels
{
    [CustomEditor(typeof(CreateWheel))]
    [CanEditMultipleObjects]
    public class CreateWheelEditor: Editor
    {
        private SerializedProperty _wheelMassProp;
        private SerializedProperty _wheelColliderRadiusProp;
        private SerializedProperty _wheelColliderMaterialProp;
        private SerializedProperty _wheelMeshProp;
        private SerializedProperty _wheelMaterialProp;
        
        private SerializedProperty _wheelDistanceProp;
        private SerializedProperty _wheelCountProp;
        private SerializedProperty _wheelSpacingProp;
        
        private Transform _transform;
        private bool _created;

        private void OnEnable()
        {
            _wheelMassProp = serializedObject.FindProperty("wheelMass");
            _wheelColliderRadiusProp = serializedObject.FindProperty("wheelColliderRadius");
            _wheelColliderMaterialProp = serializedObject.FindProperty("wheelColliderMaterial");
            _wheelMeshProp = serializedObject.FindProperty("wheelMesh");
            _wheelMaterialProp = serializedObject.FindProperty("wheelMaterial");
            
            _wheelDistanceProp = serializedObject.FindProperty("wheelDistance");
            _wheelCountProp = serializedObject.FindProperty("wheelCount");
            _wheelSpacingProp = serializedObject.FindProperty("wheelSpacing");

            _transform = ((CreateWheel) target).gameObject.transform;
            _created = true;
            
            InitWheelManager();
            
        }
        
        private void WheelMassGUI()
        {
            EditorGUILayout.PropertyField(_wheelMassProp);
        }
        
        private void WheelColliderRadiusGUI()
        {
            EditorGUILayout.PropertyField(_wheelColliderRadiusProp);
        }
        
        private void WheelColliderMaterialGUI()
        {
            EditorGUILayout.PropertyField(_wheelColliderMaterialProp);
        }
        
        private void WheelMeshGUI()
        {
            EditorGUILayout.PropertyField(_wheelMeshProp);
        }
        
        private void WheelMaterialGUI()
        {
            EditorGUILayout.PropertyField(_wheelMaterialProp);
        }
        
        private void WheelDistanceGUI()
        {
            EditorGUILayout.PropertyField(_wheelDistanceProp);
        }
        
        private void WheelCountGUI()
        {
            EditorGUILayout.PropertyField(_wheelCountProp);
        }
        
        private void WheelSpacingGUI()
        {
            EditorGUILayout.PropertyField(_wheelSpacingProp);
        }

        private void SetUpGUI()
        {
            WheelMassGUI();
            WheelColliderRadiusGUI();
            WheelColliderMaterialGUI();
            WheelMeshGUI();
            WheelMaterialGUI();
            WheelDistanceGUI();
            WheelCountGUI();
            WheelSpacingGUI();
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update ();
            
            SetUpGUI();

            if (GUI.changed || Event.current.commandName == "UndoRedoPerformed" || _created)
            {
                BulkDestroyAndCreateWheels();
                _created = false;
            }
                
            
            serializedObject.ApplyModifiedProperties ();
        }
        
        private void BulkDestroyAndCreateWheels()
        {
            BulkDestroyWheels();
            
            for (int i = 0; i < _wheelCountProp.intValue; i++)
            {
                CreateWheel(true, i);
                CreateWheel(false, i);
            }
        }

        private void BulkDestroyWheels()
        {
            // However missed adding the ConcurrentException whenever destroying
            // children using DestroyImmediate in a foreach loop should commit sudoku
            var childCount = _transform.childCount;
            for (var i = 0; i < childCount; i++)
                DestroyImmediate (_transform.GetChild(0).gameObject);
        }

        private void CreateWheel(bool left, int i)
        {
            string wheelName = left ? "Left Wheel " : "Right Wheel ";
            float wheelDistance = left ? _wheelDistanceProp.floatValue : -_wheelDistanceProp.floatValue;
            var wheel = new GameObject(wheelName + i)
            {
                transform =
                {
                    parent = _transform,
                    localPosition = new Vector3(i * _wheelSpacingProp.floatValue, 0, wheelDistance)
                },
            };
            
            var iconContent = EditorGUIUtility.IconContent("sv_label_1");
            EditorGUIUtility.SetIconForObject(wheel, (Texture2D) iconContent.image);
            
            InitCollider(wheel);
            
            if(_wheelMeshProp.objectReferenceValue != null && _wheelMaterialProp.objectReferenceValue != null)
                InitMesh(wheel);
            
            InitWheelScript(wheel);
        }

        private void InitCollider(GameObject wheel)
        {
            var wheelCollider = wheel.AddComponent<SphereCollider>();
            wheelCollider.radius = _wheelColliderRadiusProp.floatValue;
            wheelCollider.center = Vector3.zero;
            wheelCollider.material = _wheelColliderMaterialProp.objectReferenceValue as PhysicMaterial;
        }
        
        private void InitMesh(GameObject wheel)
        {
            MeshFilter wheelMesh = wheel.AddComponent<MeshFilter>();
            wheelMesh.mesh = _wheelMeshProp.objectReferenceValue as Mesh;
            MeshRenderer wheelRenderer = wheel.AddComponent<MeshRenderer>();
            wheelRenderer.material = _wheelMaterialProp.objectReferenceValue as Material;
        }
        
        private void InitWheelScript(GameObject wheel)
        {
            Wheel wheelController = wheel.AddComponent<Wheel>();
            
            wheelController.WheelManager = _transform.parent.GetComponent<WheelManager>();
        }
        
        private void InitWheelManager()
        {
            WheelManager wheelManager = _transform.GetComponent<WheelManager>();
            if (wheelManager == null)
                _transform.gameObject.AddComponent<WheelManager>();
        }
    }
}