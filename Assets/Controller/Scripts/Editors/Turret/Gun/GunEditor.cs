using System;
using Controller.Scripts.Editors.Utils;
using Controller.Scripts.Managers.ImpactCollision;
using Controller.Scripts.Managers.Turret;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

namespace Controller.Scripts.Editors.Turret.Gun
{
    [CustomEditor(typeof(Gun))]
    [CanEditMultipleObjects]
    public class GunEditor : TankComponentEditor
    {
        // Main Gun
        private SerializedProperty _mainGunMesh;
        private SerializedProperty _mainGunMaterials;

        private SerializedProperty _mainGunColliderMeshes;

        private SerializedProperty _mainGunUseBoxCollider;
        private SerializedProperty _mainGunBoxColliderChangeManually;
        private SerializedProperty _mainGunBoxColliderSize;
        private SerializedProperty _mainGunBoxColliderCenter;

        // Mantlet
        private SerializedProperty _mantletMesh;
        private SerializedProperty _mantletMaterials;

        private SerializedProperty _mantletColliderMeshes;

        private SerializedProperty _mantletUseBoxCollider;
        private SerializedProperty _mantletBoxColliderChangeManually;
        private SerializedProperty _mantletBoxColliderSize;
        private SerializedProperty _mantletBoxColliderCenter;

        private GameObject _mainGun;

        private void OnEnable()
        {
            _mainGunMesh = serializedObject.FindProperty("mainGunMesh");
            _mainGunMaterials =
                serializedObject.FindProperty("mainGunMaterials");

            _mainGunColliderMeshes =
                serializedObject.FindProperty("mainGunColliderMeshes");

            _mainGunUseBoxCollider =
                serializedObject.FindProperty("mainGunUseBoxCollider");
            _mainGunBoxColliderChangeManually =
                serializedObject.FindProperty(
                    "mainGunBoxColliderChangeManually");
            _mainGunBoxColliderCenter =
                serializedObject.FindProperty("mainGunBoxColliderCenter");
            _mainGunBoxColliderSize =
                serializedObject.FindProperty("mainGunBoxColliderSize");


            _mantletMesh = serializedObject.FindProperty("mantletMesh");
            _mantletMaterials =
                serializedObject.FindProperty("mantletMaterials");

            _mantletColliderMeshes =
                serializedObject.FindProperty("mantletColliderMeshes");

            _mantletUseBoxCollider =
                serializedObject.FindProperty("mantletUseBoxCollider");
            _mantletBoxColliderChangeManually =
                serializedObject.FindProperty(
                    "mantletBoxColliderChangeManually");
            _mantletBoxColliderCenter =
                serializedObject.FindProperty("mantletBoxColliderCenter");
            _mantletBoxColliderSize =
                serializedObject.FindProperty("mantletBoxColliderSize");

            transform = ((Gun)target).gameObject.transform;
            _mainGun = transform.Find("Main Gun").gameObject;

            Initialize();
        }

        private void Initialize()
        {
            if (!transform.GetComponent<VerticalRotation>())
            {
                VerticalRotation verticalRotation = transform.gameObject
                    .AddComponent<VerticalRotation>();
                verticalRotation.horizontalRotation = transform.parent
                    .GetComponent<HorizontalRotation>();
            }

            if (!transform.GetComponent<CollisionManager>())
                transform.gameObject.AddComponent<CollisionManager>();

            if (!_mainGun.GetComponent<CollisionManager>())
                _mainGun.AddComponent<CollisionManager>();

            try
            {
                GameObject spawnPoint =
                    _mainGun.transform.Find("Spawn Point").gameObject;
            }
            catch (NullReferenceException)
            {
                GameObject spawnPoint = new GameObject("Spawn Point");
                spawnPoint.transform.parent = _mainGun.transform;
                spawnPoint.transform.localPosition = Vector3.zero;
                spawnPoint.transform.localRotation = Quaternion.identity;
            }
        }

        public override void SetUpGUI()
        {
            GUIUtils.HeaderGUI(TurretMessages.MantletSettings);
            GUIUtils.PropFieldGUI(_mantletMesh, TurretMessages.Mesh);
            GUIUtils.PropFieldGUI(_mantletMaterials,
                TurretMessages.Materials);
            GUIUtils.PropFieldGUI(_mantletUseBoxCollider,
                TurretMessages.UseBoxCollider);
            if (_mantletUseBoxCollider.boolValue)
            {
                GUIUtils.PropFieldGUI(_mantletBoxColliderChangeManually,
                    TurretMessages.ChangeBoxColliderManually);
                if (!_mantletBoxColliderChangeManually.boolValue)
                {
                    BoxCollider boxCollider =
                        transform.GetComponent<BoxCollider>();
                    if (boxCollider != null)
                    {
                        _mantletBoxColliderCenter.vector3Value =
                            boxCollider.center;
                        _mantletBoxColliderSize.vector3Value =
                            boxCollider.size;
                    }

                    GUIUtils.PropFieldGUI(_mantletBoxColliderCenter,
                        TurretMessages.BoxColliderCenter);
                    GUIUtils.PropFieldGUI(_mantletBoxColliderSize,
                        TurretMessages.BoxColliderSize);
                }
            }
            else
                GUIUtils.PropFieldGUI(_mantletColliderMeshes,
                    TurretMessages.ColliderMeshes);


            GUIUtils.HeaderGUI(TurretMessages.MainGunSettings);
            GUIUtils.PropFieldGUI(_mainGunMesh, TurretMessages.Mesh);
            GUIUtils.PropFieldGUI(_mainGunMaterials,
                TurretMessages.Materials);
            GUIUtils.PropFieldGUI(_mainGunUseBoxCollider,
                TurretMessages.UseBoxCollider);
            if (_mainGunUseBoxCollider.boolValue)
            {
                GUIUtils.PropFieldGUI(_mainGunBoxColliderChangeManually,
                    TurretMessages.ChangeBoxColliderManually);
                if (!_mainGunBoxColliderChangeManually.boolValue)
                {
                    BoxCollider boxCollider =
                        _mainGun.GetComponent<BoxCollider>();
                    if (boxCollider != null)
                    {
                        _mainGunBoxColliderCenter.vector3Value =
                            boxCollider.center;
                        _mainGunBoxColliderSize.vector3Value =
                            boxCollider.size;
                    }

                    GUIUtils.PropFieldGUI(_mainGunBoxColliderCenter,
                        TurretMessages.BoxColliderCenter);
                    GUIUtils.PropFieldGUI(_mainGunBoxColliderSize,
                        TurretMessages.BoxColliderSize);
                }
            }
            else
                GUIUtils.PropFieldGUI(_mainGunColliderMeshes,
                    TurretMessages.ColliderMeshes);

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

            if (_mainGunUseBoxCollider.boolValue)
            {
                RemoveMeshColliders(mainGunTransform);

                if (_mainGunBoxColliderChangeManually.boolValue)
                {
                    UpdateBoxCollider(mainGunTransform);
                    return;
                }

                UpdateBoxCollider(mainGunTransform, _mainGunBoxColliderCenter,
                    _mainGunBoxColliderSize);
                return;
            }

            RemoveBoxCollider(mainGunTransform);
            UpdateMeshColliders(mainGunTransform, _mainGunColliderMeshes);
        }

        private void UpdateMantlet()
        {
            UpdateMesh(transform, _mantletMesh, _mantletMaterials);

            if (_mantletUseBoxCollider.boolValue)
            {
                RemoveMeshColliders(transform);
                if (_mantletBoxColliderChangeManually.boolValue)
                {
                    UpdateBoxCollider(transform);
                    return;
                }

                UpdateBoxCollider(transform, _mantletBoxColliderCenter,
                    _mantletBoxColliderSize);
                return;
            }

            RemoveBoxCollider(transform);
            UpdateMeshColliders(transform, _mantletColliderMeshes);
        }
    }
}