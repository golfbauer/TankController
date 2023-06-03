using System;
using Controller.Scripts.Editors.Turret.Base;
using Controller.Scripts.Editors.Utils;
using Controller.Scripts.Editors.Wheels;
using Controller.Scripts.Editors.Wheels.Chain;
using Controller.Scripts.Editors.Wheels.CreateRearWheel;
using Controller.Scripts.Editors.Wheels.DriveWheel;
using Controller.Scripts.Editors.Wheels.SupportWheel;
using Controller.Scripts.Editors.Wheels.SuspensionWheel;
using Controller.Scripts.Managers.Movement;
using Controller.Scripts.Managers.PlayerCamera;
using Controller.Scripts.Managers.PlayerCamera.CameraMovement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using CameraType = Controller.Scripts.Managers.PlayerCamera.CameraType;

namespace Controller.Scripts.Editors.Tank
{
    [CustomEditor(typeof(CreateTank))]
    [CanEditMultipleObjects]
    public class CreateTankEditor: TankComponentEditor
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
        private SerializedProperty _hullMaterials;
        
        // Collider
        private SerializedProperty _useBoxCollider;
        private SerializedProperty _hullMeshColliders;
        private SerializedProperty _hullColliderCenter;
        private SerializedProperty _hullColliderSize;

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
            _hullMaterials = serializedObject.FindProperty("hullMaterials");
            
            _useBoxCollider = serializedObject.FindProperty("useBoxCollider");
            _hullMeshColliders = serializedObject.FindProperty("hullMeshColliders");
            _hullColliderCenter = serializedObject.FindProperty("hullColliderCenter");
            _hullColliderSize = serializedObject.FindProperty("hullColliderSize");

            Initialize();
        }

        private void Initialize()
        {
            transform = ((CreateTank) target).gameObject.transform;
            
            if (transform.GetComponent<MovementManager>() == null)
                transform.gameObject.AddComponent<MovementManager>();

            if (transform.GetComponent<CameraManager>() == null)
            {
                _cameraManager = transform.gameObject.AddComponent<CameraManager>();
                _cameraManager.SetUpCamera();
            }
        }

        public override void SetUpGUI()
        {
            CreateComponentGUI();
            CreateCameraGUI();
            
            GUIUtils.HeaderGUI(TankUtilsMessages.Hull);
            GUIUtils.PropFieldGUI(_hullMesh, TankUtilsMessages.Mesh);
            GUIUtils.PropFieldGUI(_hullMaterials, TankUtilsMessages.Material);
            
            GUIUtils.HeaderGUI(TankUtilsMessages.Collider);
            GUIUtils.PropFieldGUI(_useBoxCollider, TankUtilsMessages.UseBoxCollider);
            if (_useBoxCollider.boolValue)
            {
                GUIUtils.PropFieldGUI(_hullColliderCenter, TankUtilsMessages.ColliderCenter);
                GUIUtils.PropFieldGUI(_hullColliderSize, TankUtilsMessages.ColliderSize);
            }
            else
            {                
                GUIUtils.PropFieldGUI(_hullMeshColliders, TankUtilsMessages.MeshCollider);
            }
            GUIUtils.HeaderGUI(TankUtilsMessages.Rigidbody);
            GUIUtils.PropFieldGUI(_hullMass, TankUtilsMessages.Mass);
            GUIUtils.PropFieldGUI(_hullCenterOfMass, TankUtilsMessages.CenterOfMass);
            GUIUtils.PropFieldGUI(_physicsIterations, TankUtilsMessages.PhysicsIterations);
        }
        
        private void CreateComponentGUI()
        {
            GUIUtils.HeaderGUI(TankUtilsMessages.Components);
            GUIUtils.PropFieldGUI(_wheelType, TankUtilsMessages.Component);
            
            if (GUILayout.Button(TankUtilsMessages.Create))
            {
                CreateComponent();
            }
        }
        
        private void CreateComponent()
        {
            var createComponent = (ComponentType) _wheelType.enumValueFlag;

            switch (createComponent)
            {
                case ComponentType.SupportWheel:
                    CreateComponent("Support Wheel", typeof(CreateSupportWheel));
                    break;
                
                case ComponentType.SuspensionWheel:
                    CreateComponent("Suspension Wheel", typeof(CreateSuspensionWheel));
                    break;
                
                case ComponentType.DriveWheel:
                    CreateComponent("Drive Wheel", typeof(CreateDriveWheel));
                    break;
                
                case ComponentType.RearWheel:
                    CreateComponent("Rear Wheel", typeof(CreateRearWheel));
                    break;
                
                case ComponentType.Chain:
                    CreateComponent("Chain", typeof(CreateChain));
                    break;
                
                case ComponentType.Turret:
                    CreateComponent("Turret", typeof(CreateTurret));
                    break;
            }
        }

        private void CreateComponent(string componentName, Type componentType)
        {
            GameObject wheel = new GameObject(componentName)
            {
                transform =
                {
                    parent = transform,
                    localPosition = Vector3.zero,
                    localRotation = Quaternion.identity,
                    localScale = Vector3.one
                }
            };
            wheel.AddComponent(componentType);
        }

        private void CreateCameraGUI()
        {
            if(_cameraManager == null)
                _cameraManager = transform.GetComponent<CameraManager>();
            
            _cameraManager.SetUpCamera();
            
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
                CameraType cameraType = (CameraType) _cameraType.intValue;
                _cameraManager.AddNewCameraPosition(transform, cameraType);
            }
        }

        private void CameraControllerGUI(CameraMovementController movementController, int index)
        {
            GameObject cameraPosition = movementController.gameObject;
            bool foldout = EditorPrefs.GetBool("CameraControllerFoldout" + index, false);
            foldout = EditorGUILayout.Foldout(foldout, cameraPosition.name);
            EditorPrefs.SetBool("CameraControllerFoldout" + index, foldout);

            if (foldout)
            {
                EditorGUI.indentLevel++;
                CameraType currentType = movementController.GetCameraType();
                CameraType newType = (CameraType)EditorGUILayout.EnumPopup(TankUtilsMessages.CameraType, currentType);
                if (newType != currentType)
                    _cameraManager.ReplaceCameraController(cameraPosition, newType);
                EditorGUILayout.ObjectField(TankUtilsMessages.Transform, cameraPosition.transform, typeof(Transform), true);
                EditorGUI.indentLevel--;
            }
        }

        public override void BulkUpdateComponents()
        {
            UpdateRigidbody(transform, _hullMass);
            UpdateMesh(transform, _hullMesh, _hullMaterials);
            UpdateCollider();
            
            LayerUtils.SetLayer(transform.gameObject, LayerUtils.HullLayer);
        }

        private void UpdateCollider()
        {
            if (_useBoxCollider.boolValue)
            {
                UpdateBoxCollider(transform, _hullColliderCenter, _hullColliderSize);
                RemoveMeshColliders(transform);
            }
            else
            {
                UpdateMeshColliders(transform, _hullMeshColliders);
                RemoveBoxCollider(transform);
            }
        }
    }
}