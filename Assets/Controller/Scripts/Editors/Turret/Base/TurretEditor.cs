using System.Collections.Generic;
using Controller.Scripts.Editors.Turret.Gun;
using Controller.Scripts.Editors.Utils;
using Controller.Scripts.Managers.ImpactCollision;
using Controller.Scripts.Managers.Turret;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
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
        
        private List<GameObject> _mainGuns;

        private void OnEnable()
        {
            _turretMesh = serializedObject.FindProperty("turretMesh");
            _turretMaterials = serializedObject.FindProperty("turretMaterials");
            
            _colliderMeshes = serializedObject.FindProperty("colliderMeshes");
            
            _useBoxCollider = serializedObject.FindProperty("useBoxCollider");
            
            _boxColliderChangeManually = serializedObject.FindProperty("boxColliderChangeManually");
            _boxColliderSize = serializedObject.FindProperty("boxColliderSize");
            _boxColliderCenter = serializedObject.FindProperty("boxColliderCenter");
            
            Initialize();
        }
        
        private void Initialize()
        {   
            transform = ((Turret) target).gameObject.transform;
            LayerUtils.SetLayer(transform.gameObject, LayerUtils.HullLayer);
            
            if(_mainGuns == null)
                _mainGuns = new List<GameObject>();
            
            if(transform.GetComponent<HorizontalRotation>() == null)
                transform.gameObject.AddComponent<HorizontalRotation>();
            
            if(transform.GetComponent<CollisionManager>() == null)
                transform.gameObject.AddComponent<CollisionManager>();
            
            if(transform.Find("Mantlet") == null)
                AddMantlet();
        }

        public override void SetUpGUI()
        {
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

            _mainGuns.Add(mantlet);
            
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