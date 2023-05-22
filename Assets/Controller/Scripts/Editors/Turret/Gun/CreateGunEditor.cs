using Controller.Scripts.Editors.Utils;
using Controller.Scripts.Managers.Turret;
using UnityEditor;
using UnityEngine;

namespace Controller.Scripts.Editors.Turret.Gun
{
    [CustomEditor(typeof(CreateGun))]
    [CanEditMultipleObjects]
    public class CreateGunEditor : TankComponentEditor
    {
        private SerializedProperty _useBoxCollider;
        private SerializedProperty _boxColliderChangeManually;
        private SerializedProperty _physicsMaterial;
        
        private SerializedProperty _mainGunMesh;
        private SerializedProperty _mainGunMaterials;
        private SerializedProperty _mainGunColliderMeshes;
        private SerializedProperty _mainGunBoxColliderSize;
        private SerializedProperty _mainGunBoxColliderCenter;

        private SerializedProperty _mantletMesh;
        private SerializedProperty _mantletMaterials;
        private SerializedProperty _mantletColliderMeshes;
        private SerializedProperty _mantletBoxColliderSize;
        private SerializedProperty _mantletBoxColliderCenter;

        private void OnEnable()
        {
            _useBoxCollider = serializedObject.FindProperty("useBoxCollider");
            _boxColliderChangeManually = serializedObject.FindProperty("boxColliderChangeManually");
            _physicsMaterial = serializedObject.FindProperty("physicsMaterial");
            
            _mainGunMesh = serializedObject.FindProperty("mainGunMesh");
            _mainGunMaterials = serializedObject.FindProperty("mainGunMaterials");
            _mainGunColliderMeshes = serializedObject.FindProperty("mainGunColliderMeshes");
            _mainGunBoxColliderCenter = serializedObject.FindProperty("mainGunBoxColliderCenter");
            _mainGunBoxColliderSize = serializedObject.FindProperty("mainGunBoxColliderSize");
            
            _mantletMesh = serializedObject.FindProperty("mantletMesh");
            _mantletMaterials = serializedObject.FindProperty("mantletMaterials");
            _mantletColliderMeshes = serializedObject.FindProperty("mantletColliderMeshes");
            _mantletBoxColliderCenter = serializedObject.FindProperty("mantletBoxColliderCenter");
            _mantletBoxColliderSize = serializedObject.FindProperty("mantletBoxColliderSize");
            
            transform = ((CreateGun) target).gameObject.transform;

            if (!transform.GetComponent<VerticalRotation>())
            {
                VerticalRotation verticalRotation = transform.gameObject.AddComponent<VerticalRotation>();
                verticalRotation.horizontalRotation = transform.parent.GetComponent<HorizontalRotation>();
            }
        }

        public override void SetUpGUI()
        {
            GUIUtils.HeaderGUI("General");
            GUIUtils.PropFieldGUI(_useBoxCollider, "Use Box Collider");
            GUIUtils.PropFieldGUI(_boxColliderChangeManually, "Box Collider Change Manually");
            GUIUtils.PropFieldGUI(_physicsMaterial, "Physics Material");
            
            GUIUtils.HeaderGUI("Main Gun");
            GUIUtils.PropFieldGUI(_mainGunMesh, "Main Gun Mesh");
            GUIUtils.PropFieldGUI(_mainGunMaterials, "Main Gun Materials");
            if (_useBoxCollider.boolValue)
            {
                GUIUtils.PropFieldGUI(_mainGunBoxColliderSize, "Main Gun Box Collider Size");
                GUIUtils.PropFieldGUI(_mainGunBoxColliderCenter, "Main Gun Box Collider Center");
            }
            else
                GUIUtils.PropFieldGUI(_mainGunColliderMeshes, "Main Gun Collider Meshes");
            
            GUIUtils.HeaderGUI("Mantlet");
            GUIUtils.PropFieldGUI(_mantletMesh, "Mantlet Mesh");
            GUIUtils.PropFieldGUI(_mantletMaterials, "Mantlet Materials");
            if (_useBoxCollider.boolValue)
            {
                GUIUtils.PropFieldGUI(_mantletBoxColliderSize, "Mantlet Box Collider Size");
                GUIUtils.PropFieldGUI(_mantletBoxColliderCenter, "Mantlet Box Collider Center");
            }
            else
                GUIUtils.PropFieldGUI(_mantletColliderMeshes, "Mantlet Collider Meshes");

            updateAll = GUIUtils.UpdateAllGUI();
        }

        public override void BulkUpdateComponents()
        {
            UpdateMainGun();
            UpdateMantlet();
        }

        private void UpdateMainGun()
        {
            Transform mainGunTransform = transform.Find("Main Gun");
            UpdateMesh(mainGunTransform, _mainGunMesh, _mainGunMaterials);

            if (_useBoxCollider.boolValue)
            {
                RemoveMeshColliders(mainGunTransform);
                
                if (_boxColliderChangeManually.boolValue)
                {
                    UpdateBoxCollider(mainGunTransform);
                    return;
                }
                
                UpdateBoxCollider(mainGunTransform, _mainGunBoxColliderCenter, _mainGunBoxColliderSize, _physicsMaterial);
                return;
            }
            
            RemoveBoxCollider(mainGunTransform);
            UpdateMeshColliders(mainGunTransform, _mainGunColliderMeshes, _physicsMaterial);
        }

        private void UpdateMantlet()
        {
            UpdateMesh(transform, _mantletMesh, _mantletMaterials);
            
            if (_useBoxCollider.boolValue)
            {
                RemoveMeshColliders(transform);
                if (_boxColliderChangeManually.boolValue)
                {
                    UpdateBoxCollider(transform);
                    return;
                }
                    
                UpdateBoxCollider(transform, _mantletBoxColliderCenter, _mantletBoxColliderSize, _physicsMaterial);
                return;
            }
            
            RemoveBoxCollider(transform);
            UpdateMeshColliders(transform, _mantletColliderMeshes, _physicsMaterial);
        }
    }
}