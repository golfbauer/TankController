using Controller.Scripts.Editors.Wheels.DriveWheel;
using Controller.Scripts.Managers.Wheels;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Controller.Scripts.Editors.Wheels.CreateRearWheel
{
    [CustomEditor(typeof(CreateRearWheel))]
    [CanEditMultipleObjects]
    public class CreateRearWheelEditor : CreateDriveWheelEditor
    {
        private SerializedProperty _showLabels;

        private SerializedProperty _rightWheelRotationProp;
        private SerializedProperty _leftWheelTorqueProp;
        private SerializedProperty _leftWheelAxisProp;
        
        private SerializedProperty _leftWheelRotationProp;
        private SerializedProperty _rightWheelTorqueProp;
        private SerializedProperty _rightWheelAxisProp;

        private SerializedProperty _wheelColliderRadiusProp;
        private SerializedProperty _wheelColliderMaterialProp;

        private SerializedProperty _rightWheelMeshProp;
        private SerializedProperty _rightWheelMaterialProp;
        
        private SerializedProperty _leftWheelMeshProp;
        private SerializedProperty _leftWheelMaterialProp;

        private SerializedProperty _wheelMassProp;

        private SerializedProperty _wheelDistanceProp;
        private SerializedProperty _wheelSpacingProp;

        private void OnEnable()
        {
            _showLabels = serializedObject.FindProperty("showLabels");

            _rightWheelRotationProp = serializedObject.FindProperty("rightWheelRotation");
            _rightWheelTorqueProp = serializedObject.FindProperty("rightWheelTorque");
            _rightWheelAxisProp = serializedObject.FindProperty("rightWheelHingeAxis");
            
            _leftWheelRotationProp = serializedObject.FindProperty("leftWheelRotation");
            _leftWheelTorqueProp = serializedObject.FindProperty("leftWheelTorque");
            _leftWheelAxisProp = serializedObject.FindProperty("leftWheelHingeAxis");

            _wheelColliderRadiusProp = serializedObject.FindProperty("wheelColliderRadius");
            _wheelColliderMaterialProp = serializedObject.FindProperty("wheelColliderMaterial");

            _rightWheelMeshProp = serializedObject.FindProperty("rightWheelMesh");
            _rightWheelMaterialProp = serializedObject.FindProperty("rightWheelMaterial");
            
            _leftWheelMeshProp = serializedObject.FindProperty("leftWheelMesh");
            _leftWheelMaterialProp = serializedObject.FindProperty("leftWheelMaterial");

            _wheelMassProp = serializedObject.FindProperty("wheelMass");

            _wheelDistanceProp = serializedObject.FindProperty("wheelDistance");
            _wheelSpacingProp = serializedObject.FindProperty("wheelSpacing");

            Transform = ((CreateRearWheel)target).gameObject.transform;
            
            AttachWheelManager();
        }
        
        protected override void SetUpGUI()
        {
            GUIUtils.HeaderGUI(WheelUtilsMessages.GeneralSettings);
            GUIUtils.PropFieldGUI(_showLabels);
            
            GUIUtils.HeaderGUI(WheelUtilsMessages.RightWheelSettings);
            GUIUtils.PropFieldGUI(_rightWheelRotationProp, WheelUtilsMessages.EulerRotation);
            GUIUtils.PropFieldGUI(_rightWheelTorqueProp, WheelUtilsMessages.TorqueDirection);
            GUIUtils.PropFieldGUI(_rightWheelAxisProp, WheelUtilsMessages.HingeAxis);
            GUIUtils.PropFieldGUI(_rightWheelMeshProp, WheelUtilsMessages.Mesh);
            GUIUtils.PropFieldGUI(_rightWheelMaterialProp, WheelUtilsMessages.Material);
            
            GUIUtils.HeaderGUI(WheelUtilsMessages.LeftWheelSettings);
            GUIUtils.PropFieldGUI(_leftWheelRotationProp, WheelUtilsMessages.EulerRotation);
            GUIUtils.PropFieldGUI(_leftWheelTorqueProp, WheelUtilsMessages.TorqueDirection);
            GUIUtils.PropFieldGUI(_leftWheelAxisProp, WheelUtilsMessages.HingeAxis);
            GUIUtils.PropFieldGUI(_leftWheelMeshProp, WheelUtilsMessages.Mesh);
            GUIUtils.PropFieldGUI(_leftWheelMaterialProp, WheelUtilsMessages.Material);
            
            GUIUtils.HeaderGUI(WheelUtilsMessages.GeneralWheelSettings);
            GUIUtils.PropFieldGUI(_wheelMassProp, WheelUtilsMessages.Mass);

            GUIUtils.SliderGUI(_wheelDistanceProp, 0.1f, 5f, WheelUtilsMessages.Distance);
            GUIUtils.SliderGUI(_wheelSpacingProp, 0.1f, 5f, WheelUtilsMessages.Spacing);
            
            GUIUtils.PropFieldGUI(_wheelColliderMaterialProp, WheelUtilsMessages.ColliderMaterial);
            GUIUtils.SliderGUI(_wheelColliderRadiusProp, 0.1f, 5f, WheelUtilsMessages.ColliderRadius);

            UpdateAllGUI();
        }

        protected override void BulkUpdateComponents()
        {
            BulkDestroyComponents();
            BulkCreateWheels();

            UpdateAll = false;
        }

        private void BulkCreateWheels()
        {
            CreateWheel(true);
            CreateWheel(false);
        }

        private void CreateWheel(bool isLeft)
        {
            string wheelName = isLeft ? WheelUtilsMessages.LeftWheel : WheelUtilsMessages.RightWheel;
            float wheelDistance = isLeft ? -_wheelDistanceProp.floatValue : _wheelDistanceProp.floatValue;
            Vector3 eulerRotation = isLeft ? _leftWheelRotationProp.vector3Value : _rightWheelRotationProp.vector3Value;

            var wheel = new GameObject(wheelName)
            {
                transform =
                {
                    parent = Transform,
                    localPosition = new Vector3(wheelDistance, 0, _wheelSpacingProp.floatValue),
                    localRotation = Quaternion.Euler(eulerRotation)
                },
            };

            Utils.ShowLabel(wheel, _showLabels);
            AttachComponents(wheel, isLeft);
        }

        private void AttachComponents(GameObject wheel, bool isLeft)
        {
            Vector3 torqueDirection = isLeft ? _leftWheelTorqueProp.vector3Value : _rightWheelTorqueProp.vector3Value;
            Vector3 hingeAxis = isLeft ? _leftWheelAxisProp.vector3Value : _rightWheelAxisProp.vector3Value;
            
            AttachCollider(wheel, _wheelColliderRadiusProp, _wheelColliderMaterialProp);
            AttachWheelScript(wheel, isLeft, torqueDirection);
            AttachFixedRigidbody(wheel, _wheelMassProp);
            AttachWheelHingeJoint(wheel, Transform.parent, hingeAxis);
            AttachMesh(wheel, isLeft ? _leftWheelMeshProp : _rightWheelMeshProp, isLeft ? _leftWheelMaterialProp : _rightWheelMaterialProp);
            SetLayers();
        }
    }
}