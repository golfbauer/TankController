using System;
using Controller.Scripts.ImpactCollision;
using Controller.Scripts.Movement;
using Controller.Scripts.PlayerCamera;
using Controller.Scripts.Tank.Services;
using Controller.Scripts.Utils;
using Controller.Scripts.Wheels.Chain;
using Controller.Scripts.Wheels.DriveWheel;
using Controller.Scripts.Wheels.RearWheel;
using Controller.Scripts.Wheels.SupportWheel;
using Controller.Scripts.Wheels.SuspensionWheel;
using UnityEditor;
using UnityEngine;

namespace Controller.Scripts.Tank
{
    [CustomEditor(typeof(Tank))]
    public class TankEditor : TankComponentEditor
    {
        private CameraManager _cameraManager;

        // Components
        private SerializedProperty _componentType;
        private SerializedProperty _hullCenterOfMass;
        private SerializedProperty _hullColliderCenter;
        private SerializedProperty _hullColliderSize;

        // Hull
        // Rigidbody
        private SerializedProperty _hullMass;
        private SerializedProperty _hullMaterials;

        // Mesh
        private SerializedProperty _hullMesh;
        private SerializedProperty _hullMeshColliders;
        private SerializedProperty _physicsIterations;

        // Collider
        private SerializedProperty _useBoxCollider;

        // Manager
        private SerializedProperty _useCameraManager;
        private SerializedProperty _useCollisionManager;
        private SerializedProperty _useMovementManager;

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
        
        /// <summary>
        /// Sometimes the target is not assigned on opening the prefab mode.
        /// This function will inform the user what to do in that case.
        /// </summary>
        public virtual void CheckTarget()
        {
            if (target == null)
                Debug.LogError(GeneralMessages.TargetNotAssigned);
        }

        private void Initialize()
        {
            transform = ((Tank)target).gameObject.transform;

            if (transform.GetComponent<MovementManager>() == null && _useMovementManager.boolValue)
                transform.gameObject.AddComponent<MovementManager>();

            if (transform.GetComponent<CameraManager>() == null && _useCameraManager.boolValue)
            {
                _cameraManager = transform.gameObject.AddComponent<CameraManager>();
                _cameraManager.SetUpCamera();
            }

            if (
                transform.GetComponent<CollisionManager>() == null
                && _useCollisionManager.boolValue
            )
                transform.gameObject.AddComponent<CollisionManager>();
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
                CreateComponent();
        }

        private void CreateComponent()
        {
            var createComponent = (ComponentType)_componentType.enumValueFlag;

            switch (createComponent)
            {
                case ComponentType.SupportWheel:
                    CreateComponent("Support Wheel", typeof(SupportWheel));
                    break;

                case ComponentType.SuspensionWheel:
                    CreateComponent("Suspension Wheel", typeof(SuspensionWheel));
                    break;

                case ComponentType.DriveWheel:
                    CreateComponent("Drive Wheel", typeof(DriveWheel));
                    break;

                case ComponentType.RearWheel:
                    CreateComponent("Rear Wheel", typeof(RearWheel));
                    break;

                case ComponentType.Chain:
                    CreateComponent("Chain", typeof(Chain));
                    break;

                case ComponentType.Turret:
                    CreateComponent("Turret", typeof(Editors.Turret.Base.Turret));
                    break;
            }
        }

        private void CreateComponent(string componentName, Type componentType)
        {
            var component = new GameObject(componentName)
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

        public override void ApplyUpdate()
        {
            var rigidbody = UpdateRigidbody(transform, _hullMass);
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
