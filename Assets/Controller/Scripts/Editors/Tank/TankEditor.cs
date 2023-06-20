using System;
using Controller.Scripts.Editors.Turret.Base;
using Controller.Scripts.Editors.Utils;
using Controller.Scripts.Editors.Wheels.Chain;
using Controller.Scripts.Editors.Wheels.CreateRearWheel;
using Controller.Scripts.Editors.Wheels.DriveWheel;
using Controller.Scripts.Editors.Wheels.SupportWheel;
using Controller.Scripts.Editors.Wheels.SuspensionWheel;
using Controller.Scripts.Managers.ImpactCollision;
using Controller.Scripts.Managers.Movement;
using Controller.Scripts.Managers.PlayerCamera;
using Controller.Scripts.Managers.PlayerCamera.CameraMovement;
using UnityEditor;
using UnityEngine;
using CameraType = Controller.Scripts.Managers.PlayerCamera.CameraType;

namespace Controller.Scripts.Editors.Tank
{
    [CustomEditor(typeof(Tank))]
    public class TankEditor: TankComponentEditor
    {
        // Components
        private SerializedProperty _componentType;
        
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
            CheckTarget();
            
            _componentType = serializedObject.FindProperty("componentType");
            
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
            transform = ((Tank) target).gameObject.transform;
            
            if (transform.GetComponent<MovementManager>() == null)
                transform.gameObject.AddComponent<MovementManager>();

            if (transform.GetComponent<CameraManager>() == null)
            {
                _cameraManager = transform.gameObject.AddComponent<CameraManager>();
                _cameraManager.SetUpCamera();
            }

            if (transform.GetComponent<CollisionManager>() == null)
            {
                transform.gameObject.AddComponent<CollisionManager>();
            }
        }

        public override void SetUpGUI()
        {
            CreateComponentGUI();
            CameraPositionsGUI();
            
            GUIUtils.HeaderGUI(CreateTankMessages.Hull);
            GUIUtils.PropFieldGUI(_hullMesh, CreateTankMessages.Mesh);
            GUIUtils.PropFieldGUI(_hullMaterials, CreateTankMessages.Material);
            
            GUIUtils.HeaderGUI(CreateTankMessages.Collider);
            GUIUtils.PropFieldGUI(_useBoxCollider, CreateTankMessages.UseBoxCollider);
            if (_useBoxCollider.boolValue)
            {
                GUIUtils.PropFieldGUI(_hullColliderCenter, CreateTankMessages.ColliderCenter);
                GUIUtils.PropFieldGUI(_hullColliderSize, CreateTankMessages.ColliderSize);
            }
            else
            {                
                GUIUtils.PropFieldGUI(_hullMeshColliders, CreateTankMessages.MeshCollider);
            }
            
            GUIUtils.HeaderGUI(CreateTankMessages.Rigidbody);
            GUIUtils.PropFieldGUI(_hullMass, CreateTankMessages.Mass);
            GUIUtils.PropFieldGUI(_hullCenterOfMass, CreateTankMessages.CenterOfMass);
            GUIUtils.PropFieldGUI(_physicsIterations, CreateTankMessages.PhysicsIterations);
            
            UpdateAllGUI();
        }
        
        private void CreateComponentGUI()
        {
            GUIUtils.HeaderGUI(CreateTankMessages.Components);
            GUIUtils.PropFieldGUI(_componentType, CreateTankMessages.Component);
            
            if (GUILayout.Button(CreateTankMessages.Create))
            {
                CreateComponent();
            }
        }
        
        private void CreateComponent()
        {
            var createComponent = (ComponentType) _componentType.enumValueFlag;

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
                
                case ComponentType.Camera:
                    _cameraManager.AddNewCameraPosition(transform, CameraType.ThirdPerson);
                    break;
            }
        }

        private void CreateComponent(string componentName, Type componentType)
        {
            GameObject component = new GameObject(componentName)
            {
                transform =
                {
                    parent = transform,
                    localPosition = Vector3.zero,
                    localRotation = Quaternion.identity,
                    localScale = Vector3.one
                }
            };
            component.AddComponent(componentType);
        }

        private void CameraPositionsGUI()
        {
            if(_cameraManager == null)
                _cameraManager = transform.GetComponent<CameraManager>();
            
            _cameraManager.SetUpCamera();
            
            GUIUtils.HeaderGUI(CreateTankMessages.Camera);
            
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
        }

        private void CameraControllerGUI(CameraMovementController movementController, int index)
        {
            GameObject cameraPosition = movementController.gameObject;
            bool foldout = EditorPrefs.GetBool("CameraPositionFoldout" + index, false);
            foldout = EditorGUILayout.Foldout(foldout, cameraPosition.name);
            EditorPrefs.SetBool("CameraPositionFoldout" + index, foldout);

            if (foldout)
            {
                EditorGUI.indentLevel++;
                CameraType currentType = movementController.GetCameraType();
                CameraType newType = (CameraType)EditorGUILayout.EnumPopup(CreateTankMessages.CameraType, currentType);
                if (newType != currentType)
                    _cameraManager.ReplaceCameraController(cameraPosition, newType);
                EditorGUILayout.ObjectField(CreateTankMessages.Transform, cameraPosition.transform, typeof(Transform), true);
                if (GUILayout.Button(GeneralMessages.Remove))
                    _cameraManager.RemoveCameraPosition(cameraPosition);
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