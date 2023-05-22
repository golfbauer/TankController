using System.Collections.Generic;
using Controller.Scripts.Editors.Turret.Gun;
using Controller.Scripts.Editors.Utils;
using Controller.Scripts.Managers.Turret;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Controller.Scripts.Editors.Turret.Base
{
    [CustomEditor(typeof(CreateTurret))]
    [CanEditMultipleObjects]
    public class CreateTurretEditor : TankComponentEditor
    {
        // Turret
        // Mesh
        private SerializedProperty _turretMesh;
        private SerializedProperty _turretMaterials;
        
        // Collider
        private SerializedProperty _physicsMaterial;

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
            
            _physicsMaterial = serializedObject.FindProperty("physicsMaterial");
            _useBoxCollider = serializedObject.FindProperty("useBoxCollider");
            
            _boxColliderChangeManually = serializedObject.FindProperty("boxColliderChangeManually");
            _boxColliderSize = serializedObject.FindProperty("boxColliderSize");
            _boxColliderCenter = serializedObject.FindProperty("boxColliderCenter");
            
            Initialize();
        }
        
        private void Initialize()
        {   
            transform = ((CreateTurret) target).gameObject.transform;
            LayerUtils.SetLayer(transform.gameObject, LayerUtils.HullLayer);
            
            if(_mainGuns == null)
                _mainGuns = new List<GameObject>();
            
            if(transform.GetComponent<HorizontalRotation>() == null)
                transform.gameObject.AddComponent<HorizontalRotation>();
        }

        public override void SetUpGUI()
        {
            GUIUtils.HeaderGUI("Turret");
            GUIUtils.PropFieldGUI(_turretMesh, "Turret Mesh");
            GUIUtils.PropFieldGUI(_turretMaterials, "Turret Materials");
            GUIUtils.PropFieldGUI(_useBoxCollider, "Box Collider");
            GUIUtils.PropFieldGUI(_physicsMaterial, "Physics Material");
            ShowCollider();
            
            EditorGUILayout.Space();
            AddNewMantlet();
            updateAll = GUIUtils.UpdateAllGUI();
        }

        private void ShowCollider()
        {
            if (_useBoxCollider.boolValue)
            {
                GUIUtils.PropFieldGUI(_boxColliderChangeManually, "Change Box Collider Manually");
                GUIUtils.PropFieldGUI(_boxColliderSize, "Box Collider Size");
                GUIUtils.PropFieldGUI(_boxColliderCenter, "Box Collider Center");
            }
            else
                GUIUtils.PropFieldGUI(_colliderMeshes, "Collider Meshes");
        }

        private void AddNewMantlet()
        {
            if (GUILayout.Button("Add Main Gun"))
            {
                AddMantlet(_mainGuns.Count);
            }
        }
        
        private void AddMantlet(int i)
        {
            GameObject mantlet = new GameObject("Mantlet " + i);
            mantlet.transform.SetParent(transform);
            mantlet.transform.localPosition = Vector3.zero;
            LayerUtils.SetLayer(mantlet, LayerUtils.HullLayer);
            
            mantlet.AddComponent<CreateGun>();

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
                UpdateBoxCollider(transform,_boxColliderCenter, _boxColliderSize, _physicsMaterial);
                return;
            }
            
            RemoveBoxCollider(transform);
            UpdateMeshColliders(transform, _colliderMeshes, _physicsMaterial);
        }
    }
}