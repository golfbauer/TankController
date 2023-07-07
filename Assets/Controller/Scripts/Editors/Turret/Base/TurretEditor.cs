using System;
using Controller.Scripts.Editors.Utils;
using Controller.Scripts.Managers.Ammunition;
using Controller.Scripts.Managers.ImpactCollision;
using Controller.Scripts.Managers.Turret;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Controller.Scripts.Editors.Turret.Base
{
    [CustomEditor(typeof(Turret))]
    [CanEditMultipleObjects]
    public class TurretEditor : TankComponentEditor
    {
        // Turret
        // Mesh
        private SerializedProperty _turretMesh;
        private SerializedProperty _turretMaterials;
        
        // Collider
        private SerializedProperty _colliderMeshes;
        
        private SerializedProperty _useBoxCollider;
        private SerializedProperty _boxColliderChangeManually;
        private SerializedProperty _boxColliderSize;
        private SerializedProperty _boxColliderCenter;
        
        // Manager
        private SerializedProperty _useAmmunitionManager;
        private SerializedProperty _useRotationManager;
        private SerializedProperty _useCollisionManager;

        private void OnEnable()
        {
            _turretMesh = serializedObject.FindProperty("turretMesh");
            _turretMaterials = serializedObject.FindProperty("turretMaterials");
            
            _colliderMeshes = serializedObject.FindProperty("colliderMeshes");
            
            _useBoxCollider = serializedObject.FindProperty("useBoxCollider");
            
            _boxColliderChangeManually = serializedObject.FindProperty("boxColliderChangeManually");
            _boxColliderSize = serializedObject.FindProperty("boxColliderSize");
            _boxColliderCenter = serializedObject.FindProperty("boxColliderCenter");
            
            _useAmmunitionManager = serializedObject.FindProperty("useAmmunitionManager");
            _useRotationManager = serializedObject.FindProperty("useRotationManager");
            _useCollisionManager = serializedObject.FindProperty("useCollisionManager");
            
            Initialize();
        }
        
        private void Initialize()
        {   
            transform = ((Turret) target).gameObject.transform;
            LayerUtils.SetLayer(transform.gameObject, LayerUtils.HullLayer);

            if (!transform.GetComponent<AmmunitionManager>() && _useAmmunitionManager.boolValue)
            {
                AmmunitionManager ammunitionManager = transform.AddComponent<AmmunitionManager>();
                try
                {
                    ammunitionManager.spawnPoint = transform.Find("Mantlet/Main Gun/Spawn Point").gameObject;
                }
                catch (NullReferenceException)
                {
                    Debug.LogWarning("Couldn't find the spawn point, please assign manually.");
                }

            }
            
            if(transform.GetComponent<HorizontalRotation>() == null && _useRotationManager.boolValue)
                transform.gameObject.AddComponent<HorizontalRotation>();
            
            if(transform.GetComponent<CollisionManager>() == null && _useCollisionManager.boolValue)
                transform.gameObject.AddComponent<CollisionManager>();
            
            if(transform.Find("Mantlet") == null)
                AddMantlet();
        }

        public override void SetUpGUI()
        {
            GUIUtils.HeaderGUI(TurretMessages.TurretManager);
            GUIUtils.PropFieldGUI(_useAmmunitionManager, TurretMessages.UseAmmunitionManager);
            GUIUtils.PropFieldGUI(_useRotationManager, TurretMessages.UseRotationManager);
            GUIUtils.PropFieldGUI(_useCollisionManager, TurretMessages.UseCollisionManager);
            
            GUIUtils.HeaderGUI(TurretMessages.TurretSettings);
            GUIUtils.PropFieldGUI(_turretMesh, TurretMessages.Mesh);
            GUIUtils.PropFieldGUI(_turretMaterials, TurretMessages.Materials);
            
            GUIUtils.PropFieldGUI(_useBoxCollider, TurretMessages.UseBoxCollider);
            ShowCollider();
            
            EditorGUILayout.Space();
            updateAll = GUIUtils.UpdateAllGUI();
        }

        private void ShowCollider()
        {
            if (_useBoxCollider.boolValue)
            {
                GUIUtils.PropFieldGUI(_boxColliderChangeManually, TurretMessages.ChangeBoxColliderManually);
                if (!_boxColliderChangeManually.boolValue)
                {
                    BoxCollider boxCollider = transform.GetComponent<BoxCollider>();
                    if (boxCollider != null)
                    {
                        _boxColliderCenter.vector3Value = boxCollider.center;
                        _boxColliderSize.vector3Value = boxCollider.size;
                    }
                    GUIUtils.PropFieldGUI(_boxColliderSize, TurretMessages.BoxColliderSize);
                    GUIUtils.PropFieldGUI(_boxColliderCenter, TurretMessages.BoxColliderCenter);
                }
            }
            else
                GUIUtils.PropFieldGUI(_colliderMeshes, TurretMessages.ColliderMeshes);
        }
        
        private void AddMantlet()
        {
            GameObject mantlet = new GameObject("Mantlet");
            mantlet.transform.SetParent(transform);
            mantlet.transform.localPosition = Vector3.zero;
            LayerUtils.SetLayer(mantlet, LayerUtils.HullLayer);
            
            mantlet.AddComponent<Gun.Gun>();

            GameObject gun = new GameObject("Main Gun");
            gun.transform.SetParent(mantlet.transform);
            gun.transform.localPosition = Vector3.zero;
            LayerUtils.SetLayer(gun, LayerUtils.HullLayer);
        }

        public override void BulkUpdateComponents()
        {
            UpdateMesh(transform, _turretMesh, _turretMaterials);
            if (_useBoxCollider.boolValue)
            {
                RemoveMeshColliders(transform);
                if (_boxColliderChangeManually.boolValue)
                {
                    UpdateBoxCollider(transform);
                    return;
                }
                UpdateBoxCollider(transform,_boxColliderCenter, _boxColliderSize);
                return;
            }
            
            RemoveBoxCollider(transform);
            UpdateMeshColliders(transform, _colliderMeshes);
        }
    }
}