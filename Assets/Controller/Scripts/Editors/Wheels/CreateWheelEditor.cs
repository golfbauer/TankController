using System;
using System.Collections.Generic;
using Controller.Scripts.Managers.Wheels;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Controller.Scripts.Editors.Wheels
{
    [CustomEditor(typeof(CreateWheel))]
    [CanEditMultipleObjects]
    public class CreateWheelEditor: Editor
    {
        protected SerializedProperty ShowLabels;
        protected SerializedProperty WheelMassProp;
        protected SerializedProperty WheelColliderRadiusProp;
        protected SerializedProperty WheelColliderMaterialProp;
        protected SerializedProperty WheelMeshProp;
        protected SerializedProperty WheelMaterialProp;
        
        protected SerializedProperty WheelDistanceProp;
        protected SerializedProperty WheelCountProp;
        protected SerializedProperty WheelSpacingProp;
        
        protected Transform Transform;
        protected bool Created;

        private void OnEnable()
        {
            ShowLabels = serializedObject.FindProperty("showLabels");
            WheelMassProp = serializedObject.FindProperty("wheelMass");
            WheelColliderRadiusProp = serializedObject.FindProperty("wheelColliderRadius");
            WheelColliderMaterialProp = serializedObject.FindProperty("wheelColliderMaterial");
            WheelMeshProp = serializedObject.FindProperty("wheelMesh");
            WheelMaterialProp = serializedObject.FindProperty("wheelMaterial");
            
            WheelDistanceProp = serializedObject.FindProperty("wheelDistance");
            WheelCountProp = serializedObject.FindProperty("wheelCount");
            WheelSpacingProp = serializedObject.FindProperty("wheelSpacing");

            Transform = ((CreateWheel) target).gameObject.transform;
            Created = true;
            
            InitWheelManager();
            
        }

        private void ShowLabelGUI()
        {
            EditorGUILayout.PropertyField(ShowLabels);
        }
        
        private void WheelMassGUI()
        {
            EditorGUILayout.PropertyField(WheelMassProp);
        }
        
        private void WheelColliderRadiusGUI()
        {
            EditorGUILayout.Slider(WheelColliderRadiusProp, 0.1f, 10f, "Wheel Collider Radius");
        }
        
        private void WheelColliderMaterialGUI()
        {
            EditorGUILayout.PropertyField(WheelColliderMaterialProp);
        }
        
        private void WheelMeshGUI()
        {
            EditorGUILayout.PropertyField(WheelMeshProp);
        }
        
        private void WheelMaterialGUI()
        {
            EditorGUILayout.PropertyField(WheelMaterialProp);
        }
        
        private void WheelDistanceGUI()
        {
            EditorGUILayout.Slider(WheelDistanceProp, 0.1f, 20f, "Wheel Distance");
        }
        
        private void WheelCountGUI()
        {
            EditorGUILayout.PropertyField(WheelCountProp);
        }
        
        private void WheelSpacingGUI()
        {
            EditorGUILayout.Slider(WheelSpacingProp, 0.1f, 20f, "Wheel Spacing");
        }
        
        private void DenyAccessGUI()
        {
            EditorGUILayout.HelpBox("You can only edit this prefab in a prefab stage", MessageType.Info);
        }

        protected virtual void SetUpGUI()
        {
            ShowLabelGUI();
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
            
            if (PrefabStageUtility.GetCurrentPrefabStage() == null)
            {
                DenyAccessGUI();
                return;
            }

            SetUpGUI();

            if (GUI.changed || Event.current.commandName == "UndoRedoPerformed" || Created)
            {
                BulkDestroyAndCreateWheels();
                Created = false;
            }
                
            
            serializedObject.ApplyModifiedProperties ();
        }
        
        protected virtual void BulkDestroyAndCreateWheels()
        {
            BulkDestroyWheels();
            
            for (int i = 0; i < WheelCountProp.intValue; i++)
            {
                CreateWheel(true, i);
                CreateWheel(false, i);
            }
        }

        protected virtual void BulkDestroyWheels()
        {
            // However missed adding the ConcurrentException whenever destroying
            // children using DestroyImmediate in a foreach loop should commit sudoku
            var childCount = Transform.childCount;
            for (var i = 0; i < childCount; i++)
                DestroyImmediate (Transform.GetChild(0).gameObject);
        }

        protected virtual void CreateWheel(bool left, int i)
        {
            string wheelName = left ? "Left Wheel " : "Right Wheel ";
            float wheelDistance = left ? WheelDistanceProp.floatValue : -WheelDistanceProp.floatValue;
            var wheel = new GameObject(wheelName + i)
            {
                transform =
                {
                    parent = Transform,
                    localPosition = new Vector3(i * WheelSpacingProp.floatValue, 0, wheelDistance)
                },
            };
            
            ShowWheelLabel(wheel);
            InitComponents(wheel);
        }
        
        protected virtual void ShowWheelLabel(GameObject wheel)
        {
            if (!ShowLabels.boolValue) return;
            
            var iconContent = EditorGUIUtility.IconContent("sv_label_1");
            EditorGUIUtility.SetIconForObject(wheel, (Texture2D) iconContent.image);
        }

        protected virtual void InitComponents(GameObject wheel)
        {
            InitCollider(wheel);
            InitWheelScript(wheel);
            InitRigidbody(wheel);
            InitHingeJoint(wheel);
            
            if(WheelMeshProp.objectReferenceValue != null && WheelMaterialProp.objectReferenceValue != null)
                InitMesh(wheel);
            
        }

        protected virtual void InitCollider(GameObject wheel)
        {
            var wheelCollider = wheel.AddComponent<SphereCollider>();
            wheelCollider.radius = WheelColliderRadiusProp.floatValue;
            wheelCollider.center = Vector3.zero;
            wheelCollider.material = WheelColliderMaterialProp.objectReferenceValue as PhysicMaterial;
        }
        
        protected virtual void InitMesh(GameObject wheel)
        {
            MeshFilter wheelMesh = wheel.AddComponent<MeshFilter>();
            wheelMesh.mesh = WheelMeshProp.objectReferenceValue as Mesh;
            MeshRenderer wheelRenderer = wheel.AddComponent<MeshRenderer>();
            wheelRenderer.material = WheelMaterialProp.objectReferenceValue as Material;
        }
        
        protected virtual void InitRigidbody(GameObject wheel)
        {
            Rigidbody wheelRigidbody = wheel.AddComponent<Rigidbody>();
            wheelRigidbody.mass = WheelMassProp.floatValue;
            wheelRigidbody.useGravity = true;
        }

        protected virtual void InitHingeJoint(GameObject wheel)
        {
            HingeJoint wheelHingeJoint = wheel.AddComponent<HingeJoint>();
            wheelHingeJoint.anchor = Vector3.zero;
            wheelHingeJoint.axis = Vector3.up;
            wheelHingeJoint.connectedBody = Transform.parent.GetComponent<Rigidbody>();
        }
        
        protected virtual void InitWheelScript(GameObject wheel)
        {
            Wheel wheelController = wheel.AddComponent<Wheel>();
            
            wheelController.WheelManager = Transform.parent.GetComponent<WheelManager>();
        }
        
        protected virtual void InitWheelManager()
        {
            WheelManager wheelManager = Transform.GetComponent<WheelManager>();
            if (wheelManager == null)
                Transform.gameObject.AddComponent<WheelManager>();
        }
    }
}