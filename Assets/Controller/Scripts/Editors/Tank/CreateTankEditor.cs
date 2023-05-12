using System;
using Controller.Scripts.Editors.Wheels;
using Controller.Scripts.Editors.Wheels.CreateRearWheel;
using Controller.Scripts.Editors.Wheels.DriveWheel;
using Controller.Scripts.Editors.Wheels.SupportWheel;
using Controller.Scripts.Editors.Wheels.SuspensionWheel;
using Controller.Scripts.Managers.Movement;
using Controller.Scripts.Managers.PlayerCamera;
using Controller.Scripts.Managers.PlayerCamera.CameraController;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using CameraType = Controller.Scripts.Managers.PlayerCamera.CameraType;

namespace Controller.Scripts.Editors.Tank
{
    [CustomEditor(typeof(CreateTank))]
    [CanEditMultipleObjects]
    public class CreateTankEditor: Editor
    {
        // Components
        private SerializedProperty _wheelType;
        private SerializedProperty _cameraType;
        
        // Hull
        // Rigidbody
        private SerializedProperty _hullMass;
        private SerializedProperty _physicsIterations;
        private SerializedProperty _hullCenterOfMass;
        
        // Mesh
        private SerializedProperty _hullMesh;
        private SerializedProperty _hullMaterial;
        
        // Collider
        private SerializedProperty _hullColliderCenter;
        private SerializedProperty _hullColliderSize;
        
        // Transform
        private Transform _transform;
        
        // Manager
        private CameraManager _cameraManager;
        
        private void OnEnable()
        {
            _wheelType = serializedObject.FindProperty("wheelType");
            _cameraType = serializedObject.FindProperty("cameraType");
            
            _hullMass = serializedObject.FindProperty("hullMass");
            _hullCenterOfMass = serializedObject.FindProperty("hullCenterOfMass");
            _physicsIterations = serializedObject.FindProperty("physicsIterations");
            
            _hullMesh = serializedObject.FindProperty("hullMesh");
            _hullMaterial = serializedObject.FindProperty("hullMaterial");
            
            _hullColliderCenter = serializedObject.FindProperty("hullColliderCenter");
            _hullColliderSize = serializedObject.FindProperty("hullColliderSize");

            
            _transform = ((CreateTank) target).gameObject.transform;
            
            if (_transform.GetComponent<MovementManager>() == null)
                _transform.gameObject.AddComponent<MovementManager>();
        }

        private void SetUpGUI()
        {
            CreateWheelGUI();
            CreateCameraGUI();
            
            GUIUtils.HeaderGUI(TankUtilsMessages.Hull);
            GUIUtils.PropFieldGUI(_hullMesh, TankUtilsMessages.Mesh);
            GUIUtils.PropFieldGUI(_hullMaterial, TankUtilsMessages.Material);
            
            GUIUtils.HeaderGUI(TankUtilsMessages.Collider);
            GUIUtils.PropFieldGUI(_hullColliderCenter, TankUtilsMessages.ColliderCenter);
            GUIUtils.PropFieldGUI(_hullColliderSize, TankUtilsMessages.ColliderSize);
            
            GUIUtils.HeaderGUI(TankUtilsMessages.Rigidbody);
            GUIUtils.PropFieldGUI(_hullMass, TankUtilsMessages.Mass);
            GUIUtils.PropFieldGUI(_hullCenterOfMass, TankUtilsMessages.CenterOfMass);
            GUIUtils.PropFieldGUI(_physicsIterations, TankUtilsMessages.PhysicsIterations);
        }
        
        private void CreateWheelGUI()
        {
            GUIUtils.HeaderGUI(TankUtilsMessages.Wheel);
            GUIUtils.PropFieldGUI(_wheelType, TankUtilsMessages.WheelType);
            
            if (GUILayout.Button(TankUtilsMessages.Create))
            {
                CreateWheelType();
            }
        }
        
        private void CreateWheelType()
        {
            var createComponent = (WheelType) _wheelType.enumValueFlag;

            switch (createComponent)
            {
                case WheelType.SupportWheel:
                    CreateWheel("Support Wheel", typeof(CreateSupportWheel));
                    break;
                
                case WheelType.SuspensionWheel:
                    CreateWheel("Suspension Wheel", typeof(CreateSuspensionWheel));
                    break;
                
                case WheelType.DriveWheel:
                    CreateWheel("Drive Wheel", typeof(CreateDriveWheel));
                    break;
                
                case WheelType.RearWheel:
                    CreateWheel("Rear Wheel", typeof(CreateRearWheel));
                    break;
            }
        }

        private void CreateWheel(string wheelName, Type wheelType)
        {
            GameObject wheel = new GameObject(wheelName)
            {
                transform =
                {
                    parent = _transform,
                    localPosition = Vector3.zero,
                    localRotation = Quaternion.identity,
                    localScale = Vector3.one
                }
            };
            wheel.AddComponent(wheelType);
        }
        
        private void CreateCameraGUI()
        {
            if(_cameraManager == null)
                _cameraManager = _transform.GetComponent<CameraManager>();
            
            GUIUtils.HeaderGUI(TankUtilsMessages.Camera);
            
            if (_cameraManager != null)
            {
                for(int i=0; i < _cameraManager.GetCameraControllers().Count; i++)
                {
                    var cameraController = _cameraManager.GetCameraControllers()[i];
                    if (cameraController == null)
                        continue;
                    CameraControllerGUI(cameraController, i);
                }
            }
            EditorGUILayout.Space();
            
            GUIUtils.PropFieldGUI(_cameraType, TankUtilsMessages.CameraType);
            
            if (GUILayout.Button(TankUtilsMessages.Create))
            {
                if (_transform.GetComponent<CameraManager>() == null)
                    _cameraManager = _transform.gameObject.AddComponent<CameraManager>();
                CameraType cameraType = (CameraType) _cameraType.intValue;
                _cameraManager.AddNewCameraPosition(_transform, cameraType);
            }
        }

        private void CameraControllerGUI(CameraController controller, int index)
        {
            GameObject cameraPosition = controller.gameObject;
            bool foldout = EditorPrefs.GetBool("CameraControllerFoldout" + index, false);
            foldout = EditorGUILayout.Foldout(foldout, cameraPosition.name);
            EditorPrefs.SetBool("CameraControllerFoldout" + index, foldout);

            if (foldout)
            {
                EditorGUI.indentLevel++;
                CameraType currentType = controller.GetCameraType();
                CameraType newType = (CameraType)EditorGUILayout.EnumPopup(TankUtilsMessages.CameraType, currentType);
                if (newType != currentType)
                    _cameraManager.ReplaceCameraController(cameraPosition, newType);
                EditorGUILayout.ObjectField(TankUtilsMessages.Transform, cameraPosition.transform, typeof(Transform), true);
                EditorGUI.indentLevel--;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update ();
            
            if (PrefabStageUtility.GetCurrentPrefabStage() == null)
            {
                GUIUtils.DenyAccessGUI();
                return;
            }

            SetUpGUI();
            
            if (GUI.changed || Event.current.commandName == "UndoRedoPerformed")
                UpdateAll();

            serializedObject.ApplyModifiedProperties();
        }

        private void UpdateAll()
        {
            UpdateRigidbody();
            UpdateMesh();
            UpdateMeshMaterial();
            UpdateCollider();
            RefreshParentSelection(_transform.gameObject);
            
            LayersUtils.SetLayer(_transform.gameObject, LayersUtils.HullLayer);
        }

        private void UpdateRigidbody()
        {
            Rigidbody rigidbody = _transform.GetComponent<Rigidbody>();
            if (rigidbody == null)
                rigidbody = _transform.gameObject.AddComponent<Rigidbody>();
            
            rigidbody.mass = _hullMass.floatValue;
        }
        
        private void UpdateMesh()
        {
            MeshFilter meshFilter = _transform.GetComponent<MeshFilter>();
            if (meshFilter == null)
                meshFilter = _transform.gameObject.AddComponent<MeshFilter>();
            
            meshFilter.mesh = (Mesh) _hullMesh.objectReferenceValue;
        }
        
        private void UpdateMeshMaterial()
        {
            MeshRenderer meshRenderer = _transform.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
                meshRenderer = _transform.gameObject.AddComponent<MeshRenderer>();

            Material[] materials = new Material[_hullMaterial.arraySize];
            for (int i = 0; i < _hullMaterial.arraySize; i++)
            {
                materials[i] = (Material)_hullMaterial.GetArrayElementAtIndex(i).objectReferenceValue;
            }

            meshRenderer.materials = materials;
        }

        private void UpdateCollider()
        {
            BoxCollider boxCollider = _transform.GetComponent<BoxCollider>();
            if (boxCollider == null)
                boxCollider = _transform.gameObject.AddComponent<BoxCollider>();
            
            boxCollider.center = _hullColliderCenter.vector3Value;
            boxCollider.size = _hullColliderSize.vector3Value;
        }

        private void RefreshParentSelection(GameObject gameObject)
        {
            Selection.activeGameObject = null;
            Selection.activeGameObject = gameObject;
        }
    }
}