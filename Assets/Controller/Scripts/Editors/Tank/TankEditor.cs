using System;
using Controller.Scripts.Editors.Utils;
using Controller.Scripts.Editors.Wheels.Chain;
using Controller.Scripts.Editors.Wheels.CreateRearWheel;
using Controller.Scripts.Editors.Wheels.DriveWheel;
using Controller.Scripts.Editors.Wheels.SupportWheel;
using Controller.Scripts.Editors.Wheels.SuspensionWheel;
using Controller.Scripts.Managers.ImpactCollision;
using Controller.Scripts.Managers.Movement;
using Controller.Scripts.Managers.PlayerCamera.CameraMovement;
using UnityEditor;
using UnityEngine;

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
        private SerializedProperty _useCameraManager;
        private SerializedProperty _useCollisionManager;
        private SerializedProperty _useMovementManager;
        
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
            
            _useCameraManager = serializedObject.FindProperty("useCameraManager");
            _useCollisionManager = serializedObject.FindProperty("useCollisionManager");
            _useMovementManager = serializedObject.FindProperty("useMovementManager");

            Initialize();
        }

        private void Initialize()
        {
            transform = ((Tank) target).gameObject.transform;
            
            if (transform.GetComponent<MovementManager>() == null && _useMovementManager.boolValue)
                transform.gameObject.AddComponent<MovementManager>();

            if (transform.GetComponent<CameraManager>() == null && _useCameraManager.boolValue)
            {
                _cameraManager = transform.gameObject.AddComponent<CameraManager>();
                _cameraManager.SetUpCamera();
            }

            if (transform.GetComponent<CollisionManager>() == null && _useCollisionManager.boolValue)
            {
                transform.gameObject.AddComponent<CollisionManager>();
            }
        }

        public override void SetUpGUI()
        {
            GUIUtils.HeaderGUI(CreateTankMessages.TankManager);
            GUIUtils.PropFieldGUI(_useCameraManager, CreateTankMessages.UseCameraManager);
            GUIUtils.PropFieldGUI(_useCollisionManager, CreateTankMessages.UseCollisionManager);
            GUIUtils.PropFieldGUI(_useMovementManager, CreateTankMessages.UseMovementManager);
            
            CreateComponentGUI();
            
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
                    CreateComponent("Turret", typeof(Turret.Base.Turret));
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

        public override void BulkUpdateComponents()
        {
            Rigidbody rigidbody = UpdateRigidbody(transform, _hullMass);
            rigidbody.centerOfMass = _hullCenterOfMass.vector3Value;
            rigidbody.solverIterations = _physicsIterations.intValue;
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